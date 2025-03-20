// DOM Elements
const bubble = document.getElementById("interviewerAIBubble");
const stateDisplay = document.getElementById("interviewerAIState");
const repeatQuestionBtn = document.getElementById("repeatQuestion");
const startAnswerBtn = document.getElementById("startAnswer");
const endAnswerBtn = document.getElementById("endAnswer");
const skipQuestionBtn = document.getElementById("skipQuestion");
const pauseInterviewBtn = document.getElementById("pauseInterview");
const prepareInterviewBtn = document.getElementById("prepareInterview");
const endInterviewBtn = document.querySelector(".endInterviewBtn");
const questionNumDiv = document.getElementById("questionNum");
const questionTimer = document.getElementById("questionTimer");
const questionText = document.getElementById("questionText");
const toneElement = document.getElementById("toneElement");

// Variables 
let audioContext, analyzer, source, dataArray;
let appState = "جاهز"; // "جاهز" (ready), "يتكلم" (speaking), "يفكر" (thinking), "يستمع" (listening)
let timer;
let timeLeft = 0; // Will be set based on estimatedTimeMinutes
let accumulatedText = "";
let currentQuestionIndex = 0;
let animationFrameId = null;
let sessionData = null; // Stores API response
let questions = []; // Populated from API
let answers = []; // Stores user answers
let repeatClickCount = 0; // Tracks "Repeat Question" clicks
let isWaitingForApiResponse = true; // Track API response state
let isFailed = false; // If the request failed state
let isReady = false; // 
let isEvaluating = false;
let answerStartTime = null;
let isProcessingEnd = false;
let isPaused = false;
let pausedStateDisplayText = "متوقف";

let arabicVoice = null;
let voicesLoadedPromise = null;

let voiceGender = "female";
let language = toneElement.textContent.substring(0,2);
let accent = toneElement.textContent;


console.log(language, accent);

// Speech Recognition Setup
const recognition = new (window.SpeechRecognition || window.webkitSpeechRecognition)();
recognition.lang = accent; // Arabic language for recognition
recognition.continuous = true;
recognition.interimResults = true;

// Setup Audio Analyzer
function setupAudioAnalyzer() {
    if (!audioContext) {
        audioContext = new (window.AudioContext || window.webkitAudioContext)();
        analyzer = audioContext.createAnalyser();
        analyzer.fftSize = 256;
        dataArray = new Uint8Array(analyzer.frequencyBinCount);
    }
    if (audioContext.state === "suspended") {
        audioContext.resume().then(() => console.log("AudioContext resumed"));
    }
}

// Update State Display and Toggle Buttons
function updateStateDisplay() {
    if (isPaused) {
        if (appState === "يستمع" || (appState === "جاهز" && currentQuestionIndex < questions.length)) {
            stateDisplay.textContent = "متوقف";
        } else {
            stateDisplay.textContent = pausedStateDisplayText || stateDisplay.textContent;
        }
    } else if (isFailed) {
        stateDisplay.textContent = "فشل في بدء المقابلة";
    } else if (appState === "يستمع") {
        stateDisplay.textContent = "يستمع";
    } else if (appState === "يفكر") {
        if (isEvaluating) {
            stateDisplay.textContent = "جاري تقييم النتيجة";
        } else if (isWaitingForApiResponse) {
            stateDisplay.textContent = "جاري تجهيز المقابلة";
        } else if (currentQuestionIndex >= questions.length) {
            stateDisplay.textContent = "جاري تقييم مقابلتك...";
        } else {
            stateDisplay.textContent = "يفكر";   
        }   
    } else if (appState === "يتكلم") {
        stateDisplay.textContent = "يتكلم";
    } else if (isReady) {
        stateDisplay.textContent = "بدء المقابلة";
        isReady = false;
    } else if (appState === "جاهز" && currentQuestionIndex < questions.length) {
        stateDisplay.textContent = "اضغط لبدء الإجابة";
        bubble.style.transform = "scale(1)";
        bubble.classList.remove("speaking", "processing", "listening");
    } else {
        stateDisplay.textContent = "جاهز";
        bubble.style.transform = "scale(1)";
        bubble.classList.remove("speaking", "processing", "listening");
    }
    toggleButtons(); // Call toggleButtons to update button states
}
// Toggle Button States
function toggleButtons() {
    const isIdle = appState === "جاهز";
    const questionCount = Array.isArray(questions) ? questions.length : 0;
    repeatQuestionBtn.disabled = !isIdle || currentQuestionIndex >= questionCount || sessionData === null;
    startAnswerBtn.disabled = !isIdle || currentQuestionIndex >= questionCount || sessionData === null;
    endAnswerBtn.disabled = appState !== "يستمع";
    skipQuestionBtn.disabled = !isIdle || currentQuestionIndex >= questionCount || sessionData === null;

    const pauseSvg = `<svg width="25" height="24" viewBox="0 0 25 24" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M9.14 21.75H5.51C3.65 21.75 2.75 20.89 2.75 19.11V4.89C2.75 3.11 3.65 2.25 5.51 2.25H9.14C11 2.25 11.9 3.11 11.9 4.89V19.11C11.9 20.89 11 21.75 9.14 21.75ZM5.51 3.75C4.43 3.75 4.25 4.02 4.25 4.89V19.11C4.25 19.98 4.42 20.25 5.51 20.25H9.14C10.22 20.25 10.4 19.98 10.4 19.11V4.89C10.4 4.02 10.23 3.75 9.14 3.75H5.51Z" fill="#F4F4F4"/>
        <path d="M19.4901 21.75H15.8601C14.0001 21.75 13.1001 20.89 13.1001 19.11V4.89C13.1001 3.11 14.0001 2.25 15.8601 2.25H19.4901C21.3501 2.25 22.2501 3.11 22.2501 4.89V19.11C22.2501 20.89 21.3501 21.75 19.4901 21.75ZM15.8601 3.75C14.7801 3.75 14.6001 4.02 14.6001 4.89V19.11C14.6001 19.98 14.7701 20.25 15.8601 20.25H19.4901C20.5701 20.25 20.7501 19.98 20.7501 19.11V4.89C20.7501 4.02 20.5801 3.75 19.4901 3.75H15.8601Z" fill="#F4F4F4"/>
    </svg>`;

    const resumeSvg = `<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M4 11.9999V8.43989C4 4.01989 7.13 2.2099 10.96 4.4199L14.05 6.1999L17.14 7.9799C20.97 10.1899 20.97 13.8099 17.14 16.0199L14.05 17.7999L10.96 19.5799C7.13 21.7899 4 19.9799 4 15.5599V11.9999Z" stroke="#F4F4F4" stroke-width="1.5" stroke-miterlimit="10" stroke-linecap="round" stroke-linejoin="round"/>
    </svg>`;

    if (appState === "يستمع" || appState === "يتكلم" || appState === "يفكر") {
        pauseInterviewBtn.disabled = false;
    } else {
        pauseInterviewBtn.disabled = true;
    }
    pauseInterviewBtn.setAttribute("data-action", isPaused ? "استئناف المقابلة" : "إيقاف المقابلة");
    pauseInterviewBtn.innerHTML = isPaused ? resumeSvg : pauseSvg;
}

// Animate Listening Bubble
function animateListeningBubble() {
    function step() {
        if (appState !== "يستمع" || !source || isPaused) {
            bubble.style.transform = "scale(1)";
            bubble.classList.remove("listening");
            cancelAnimationFrame(animationFrameId);
            animationFrameId = null;
            return;
        }
        bubble.classList.add("listening");
        bubble.classList.remove("speaking", "processing");
        analyzer.getByteFrequencyData(dataArray);
        const average = dataArray.reduce((acc, val) => acc + val, 0) / dataArray.length;
        const rawScale = average > 10 ? 1 + average / 100 : 1;
        const minScale = 1;
        const maxScale = 1.4;
        const clampedScale = Math.min(Math.max(rawScale, minScale), maxScale);
        bubble.style.transform = `scale(${clampedScale})`;
        animationFrameId = requestAnimationFrame(step);
    }
    if (animationFrameId) cancelAnimationFrame(animationFrameId);
    bubble.style.transform = "scale(1)";
    bubble.classList.add("listening");
    animationFrameId = requestAnimationFrame(step);
}

recognition.onstart = () => {
    appState = "يستمع";
    updateStateDisplay();
    bubble.classList.remove("speaking", "processing");
    navigator.mediaDevices
        .getUserMedia({ audio: true })
        .then((stream) => {
            setupAudioAnalyzer();
            source = audioContext.createMediaStreamSource(stream);
            source.connect(analyzer);
            animateListeningBubble();
        })
        .catch((err) => console.error("Audio stream error:", err));
};

recognition.onresult = (event) => {
    let interimTranscript = '';
    for (let i = event.resultIndex; i < event.results.length; i++) {
        const transcript = event.results[i][0].transcript;
        if (event.results[i].isFinal) {
            accumulatedText += transcript + ' ';
        } else {
            interimTranscript += transcript;
        }
    }
    console.log("Interim:", interimTranscript);
    console.log("Accumulated so far:", accumulatedText);
};

recognition.onend = () => {
    if (isPaused) {
        console.log("Recognition paused - onend callback skipped.");
        return;
    }
    if (isProcessingEnd) {
        console.log("Recognition stopped due to already processing");
        return;
    }

    isProcessingEnd = true;

    const answerEndTime = new Date().getTime();
    const timeTaken = answerStartTime ? (answerEndTime - answerStartTime) / 1000 : 0;
    appState = "يفكر";
    updateStateDisplay();
    if (source) {
        source.disconnect();
        source = null;
    }
    if (animationFrameId) {
        cancelAnimationFrame(animationFrameId);
        animationFrameId = null;
    }
    clearInterval(timer);
    bubble.classList.remove("listening", "speaking");
    bubble.classList.add("processing");
    bubble.innerHTML = "";

    const text = accumulatedText ? accumulatedText.trim() : "";
    answers[currentQuestionIndex] = {
        Answer: text || "",
        TimeToken: isNaN(timeTaken) ? 0 : timeTaken
    };
    console.log("Stored answer at index", currentQuestionIndex, ":", answers[currentQuestionIndex]);

    accumulatedText = "";
    repeatClickCount = 0;

    (async () => {
        if (currentQuestionIndex < questions.length - 1) {
            currentQuestionIndex++;
            questionNumDiv.textContent = `${currentQuestionIndex + 1}/${questions.length}`;
            await think(2000);
            const question = questions[currentQuestionIndex];
            const estimatedTimeSeconds = question.estimatedTimeMinutes * 60;
            questionTimer.textContent = formatTime(estimatedTimeSeconds);
            const questionTextContent = (question.linkingPhrase ? question.linkingPhrase + ", " : "") + question.originalQuestion;
            await speakText(questionTextContent); // Remove questionText.textContent assignment
        } else {
            questionNumDiv.style.display = "none";
            questionTimer.textContent = "00:00";
            questionText.textContent = "";
            const conclusionText = sessionData?.conclusionText || "حسناً، سأقوم بتقييم مقابلتك الآن، شكراً لاستخدام موقع مقابلتي!";
            await speakText(conclusionText);

            appState = "يفكر";
            isEvaluating = true;
            bubble.classList.remove("speaking", "listening");
            bubble.classList.add("processing");
            updateStateDisplay();

            await submitAnswers(answers);
        }
        answerStartTime = null;
        isProcessingEnd = false;
    })();
};

recognition.onerror = (event) => {
    stateDisplay.textContent = "خطأ: " + event.error;
    appState = "جاهز";
    updateStateDisplay();
    bubble.classList.remove("processing", "listening", "speaking");
    if (animationFrameId) {
        cancelAnimationFrame(animationFrameId);
        animationFrameId = null;
    }
};

// Start Timer
function startTimer(resume = false) {
    const question = questions[currentQuestionIndex];
    if (!resume) {
        timeLeft = (question.estimatedTimeMinutes || 1) * 60;
    }
    questionTimer.textContent = formatTime(timeLeft);
    timer = setInterval(() => {
        if (!isPaused) {
            timeLeft--;
            questionTimer.textContent = formatTime(timeLeft);
            if (timeLeft <= 0) {
                clearInterval(timer);
                recognition.stop();
            }
        }
    }, 1000);
}

// Helper function to format time in MM:SS
function formatTime(seconds) {
    const minutes = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
}

// Set default timer display
questionTimer.textContent = "00:00";

// Display Countdown Timer (5, 4, 3, 2, 1)
function displayCountdownTimer() {
    return new Promise((resolve) => {
        let countdown = 5;
        appState = "جاهز";
        bubble.classList.remove("processing", "speaking", "listening");
        bubble.innerHTML = `<div class="countdown-number">${countdown}</div>`;
        stateDisplay.textContent = "بدء المقابلة";
        isReady = true;
        toggleButtons();

        const countdownInterval = setInterval(() => {
            countdown--;
            bubble.innerHTML = `<div class="countdown-number">${countdown}</div>`;
            if (countdown <= 0) {
                clearInterval(countdownInterval);
                bubble.innerHTML = "";
                appState = "جاهز";
                updateStateDisplay();
                setTimeout(() => {
                    resolve();
                }, 100);
            }
        }, 1000);
    });
}

// Load Arabic Voice with Enhanced Selection
function loadArabicVoice() {
    const voices = speechSynthesis.getVoices();
    let selectedVoice = null;

    if (language === "ar") {
        // For Arabic, look for a voice with the specified accent (e.g., ar-EG)
        selectedVoice = voices.find(voice =>
            voice.lang === accent &&
            voice.name.toLowerCase().includes(voiceGender === "male" ? "male" : "female") &&
            (voice.name.includes("Natural") || voice.name.includes("Premium"))
        ) || voices.find(voice => voice.lang === accent && !voice.name.includes("Basic"))
            || voices.find(voice => voice.lang.startsWith("ar"))
            || (voices.length > 0 ? voices[0] : null);
    } else {
        // For English, look for a voice with an English language setting (using a US accent as default)
        selectedVoice = voices.find(voice =>
            voice.lang.startsWith("en") &&
            voice.name.toLowerCase().includes(voiceGender === "male" ? "male" : "female") &&
            (voice.name.includes("Natural") || voice.name.includes("Premium"))
        ) || voices.find(voice => voice.lang.startsWith("en") && !voice.name.includes("Basic"))
            || voices.find(voice => voice.lang.startsWith("en"))
            || (voices.length > 0 ? voices[0] : null);
    }

    // Save the selected voice to the global variable
    arabicVoice = selectedVoice;

    if (arabicVoice) {
        console.log(`Loaded voice: ${arabicVoice.name}, lang: ${arabicVoice.lang}, gender hint: ${voiceGender}`);
    } else {
        console.error("No suitable voices available for speech synthesis.");
    }
}


// Speak Text with Enhanced Human-Like Quality
async function speakText(text) {
    if (!text) return;

    // Ensure voices are loaded
    await waitForVoices();
    if (!arabicVoice) {
        console.error("No voice available to speak text:", text);
        return;
    }

    // Split text into sentences based on periods
    const sentences = text.split(/(?<=\.)\s+/).filter(s => s.trim().length > 0);

    // Process each sentence one by one
    for (const sentence of sentences) {
        // Display the current sentence
        questionText.textContent = sentence;

        // Update the app state to "speaking"
        appState = "يتكلم"; // "Speaking" in Arabic
        updateStateDisplay();
        bubble.classList.remove("processing", "listening");
        bubble.classList.add("speaking");
        bubble.innerHTML = '<div class="dot"></div><div class="dot"></div><div class="dot"></div>';

        // Speak the sentence and wait for it to finish
        await speakSentence(sentence);

        // Clear the display after speaking
        questionText.textContent = '';
        bubble.classList.remove("speaking");
    }

    // Reset to "ready" state after all sentences are spoken
    appState = "جاهز"; // "Ready" in Arabic
    bubble.style.transform = "scale(1)";
    updateStateDisplay();
    bubble.classList.remove("processing", "listening");
    bubble.innerHTML = "";
}


// Helper function to speak a sentence and wait for it to finish
function speakSentence(sentence) {
    return new Promise((resolve) => {
        const utterance = new SpeechSynthesisUtterance(sentence);
        utterance.lang = accent;
        utterance.voice = arabicVoice;
        utterance.volume = 1.0;
        utterance.rate = 0.85; // Slightly slower for natural pacing
        utterance.pitch = voiceGender === "female" ? 1.1 : 0.95; // Pitch based on gender

        utterance.onend = () => resolve();
        utterance.onerror = (event) => {
            console.error("Speech synthesis error:", event.error);
            resolve();
        };

        speechSynthesis.speak(utterance);
    });
}


// Helper function to speak a chunk and return a promise
function speakChunk(chunk) {
    return new Promise((resolve) => {
        const utterance = new SpeechSynthesisUtterance(chunk);
        utterance.lang = accent;
        utterance.voice = arabicVoice;
        utterance.volume = 1.0;
        utterance.rate = 0.85; // Slightly slower for natural pacing
        utterance.pitch = voiceGender === "female" ? 1.1 : 0.95; // Gender-based pitch

        utterance.onend = () => resolve();
        utterance.onerror = (event) => {
            console.error("Speech synthesis error:", event.error);
            resolve();
        };

        speechSynthesis.speak(utterance);
    });
}

// Wait for voices to load

function waitForVoices() {
    if (!voicesLoadedPromise) {
        voicesLoadedPromise = new Promise((resolve) => {
            const checkVoices = () => {
                const voices = speechSynthesis.getVoices();
                if (voices.length > 0) {
                    console.log("Available voices:", voices.map(voice => ({
                        name: voice.name,
                        lang: voice.lang,
                        default: voice.default
                    })));
                    loadArabicVoice();  // This now loads based on language and gender
                    resolve();
                    return;
                }
                speechSynthesis.onvoiceschanged = checkVoices;
            };
            checkVoices();
        });
    }
    return voicesLoadedPromise;
}

// Think with Pause Support
function think(duration) {
    return new Promise((resolve) => {
        appState = "يفكر";
        updateStateDisplay();
        bubble.classList.remove("speaking", "listening");
        bubble.classList.add("processing");
        const startTime = Date.now();
        function checkTime() {
            const elapsed = Date.now() - startTime;
            if (elapsed >= duration) {
                bubble.classList.remove("processing", "speaking", "listening");
                resolve();
            } else {
                setTimeout(checkTime, 100);
            }
        }
        checkTime();
    });
}

// Proceed to Next Question
async function proceedToNextQuestion() {
    if (currentQuestionIndex < questions.length) {
        await think(2000);
        const question = questions[currentQuestionIndex];
        const estimatedTimeSeconds = question.estimatedTimeMinutes * 60;
        questionTimer.textContent = formatTime(estimatedTimeSeconds);
        const questionTextContent = (question.linkingPhrase ? question.linkingPhrase + ", " : "") + question.originalQuestion;
        questionText.textContent = questionTextContent;
        await speakText(questionTextContent);
    }
}


// Button Handlers
async function startInterview() {
    if (!sessionData) return;

    await waitForVoices();
    await displayCountdownTimer();
    const introText = sessionData?.introText || "مرحباً، شكراً لانضمامك إلى المقابلة.";
    await speakText(introText);
    await think(1000);

    const firstQuestion = questions[0];
    const firstQuestionText = (firstQuestion.linkingPhrase ? firstQuestion.linkingPhrase + ", " : "") + firstQuestion.originalQuestion;
    questionNumDiv.textContent = `1/${questions.length}`;
    await speakText(firstQuestionText); // Remove questionText.textContent assignment
    const estimatedTimeSeconds = firstQuestion.estimatedTimeMinutes * 60;
    questionTimer.textContent = formatTime(estimatedTimeSeconds);
}

startAnswerBtn.onclick = () => {
    if (appState !== "جاهز" || currentQuestionIndex >= questions.length || sessionData === null) return;
    accumulatedText = "";
    answerStartTime = new Date().getTime();
    startTimer();
    recognition.start();
};

endAnswerBtn.onclick = () => {
    if (appState !== "يستمع") return;
    clearInterval(timer);
    recognition.stop();
};

repeatQuestionBtn.onclick = async () => {
    if (appState !== "جاهز" || currentQuestionIndex >= questions.length || sessionData === null) return;
    const question = questions[currentQuestionIndex];

    repeatClickCount++;
    if (repeatClickCount % 2 === 1) {
        const rephrased = question.rephrasedQuestion || "لم يتم توفير سؤال معاد صياغته.";
        await speakText(rephrased); // Remove questionText.textContent assignment
    } else {
        const explanation = question.explanation || "لم يتم توفير تفسير لهذا السؤال.";
        await speakText(explanation); // Remove questionText.textContent assignment
    }
};

skipQuestionBtn.onclick = () => {
    if (appState !== "جاهز" || currentQuestionIndex >= questions.length || sessionData === null) return;

    const answerEndTime = new Date().getTime();
    const timeTaken = answerStartTime ? (answerEndTime - answerStartTime) / 1000 : 0;
    answers[currentQuestionIndex] = {
        Answer: "",
        TimeToken: isNaN(timeTaken) ? 0 : timeTaken
    };
    console.log("Skipped answer at index", currentQuestionIndex, ":", answers[currentQuestionIndex]);

    isProcessingEnd = true;
    appState = "يفكر";
    updateStateDisplay();
    if (source) {
        source.disconnect();
        source = null; 
    }
    if (animationFrameId) {
        cancelAnimationFrame(animationFrameId);
        animationFrameId = null;
    }
    clearInterval(timer);
    bubble.classList.remove("listening", "speaking");
    bubble.classList.add("processing");
    bubble.innerHTML = "";

    (async () => {
        accumulatedText = "";
        repeatClickCount = 0;
        if (currentQuestionIndex < questions.length - 1) {
            currentQuestionIndex++;
            questionNumDiv.textContent = `${currentQuestionIndex + 1}/${questions.length}`;
            await think(2000);
            const question = questions[currentQuestionIndex];
            const estimatedTimeSeconds = question.estimatedTimeMinutes * 60;
            questionTimer.textContent = formatTime(estimatedTimeSeconds);
            const questionTextContent = (question.linkingPhrase ? question.linkingPhrase + ", " : "") + question.originalQuestion;
            await speakText(questionTextContent);
        } else {
            questionNumDiv.style.display = "none";
            questionTimer.textContent = "00:00";
            questionText.textContent = "";
            const conclusionText = sessionData?.conclusionText || "حسناً، سأقوم بتقييم مقابلتك الآن، شكراً لاستخدام موقع مقابلتي!";
            await speakText(conclusionText);

            appState = "يفكر";
            isEvaluating = true;
            bubble.classList.remove("speaking", "listening");
            bubble.classList.add("processing");
            updateStateDisplay();

            await submitAnswers(answers);
        }
        answerStartTime = null;
        isProcessingEnd = false;
    })();
};

pauseInterviewBtn.onclick = () => {
    if (!isPaused) {
        isPaused = true;
        pauseInterviewBtn.innerHTML = "Resume";

        if (appState === "يستمع") {
            recognition.stop();
        }
        if (appState === "يتكلم") {
            speechSynthesis.pause();
        }
        if (timer) {
            clearInterval(timer);
            timer = null;
        }
        if (animationFrameId) {
            cancelAnimationFrame(animationFrameId);
            animationFrameId = null;
        }
    } else {
        isPaused = false;
        pauseInterviewBtn.innerHTML = "Pause";

        if (appState === "يستمع") {
            startTimer(true);
            recognition.start();
            animateListeningBubble();
        }
        if (appState === "يتكلم") {
            speechSynthesis.resume();
        }
    }
    toggleButtons();
};

endInterviewBtn.addEventListener("click", (e) => {
    e.preventDefault(); 

    Swal.fire({
        title: "هل أنت متأكد؟هل انت متأكد من إنهاء المقابلة؟",
        text: "لن تتمكن من التراجع عن هذا الإجراء!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "إنهاء",
        cancelButtonText: "تراجع",
        width: "40%", 
        customClass: {
            popup: "custom-swal", 
            title: "custom-title",
            htmlContainer: "custom-text",
            confirmButton: "btn btn-sm btn-g1",
            cancelButton: "btn btn-sm btn-gray"
        }
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/api/Customer/interview/end',
                type: 'POST',
                success: function (response) {
                    if (response.success) {

                        window.location.href = '/customer/home/index';
                    } else {
                        toastr.error(response.message || "حدث خطأ غير متوقع.");
                    }
                },
                error: function (xhr) {
                    // Handle server-side error (status 500, etc.)
                    const response = xhr.responseJSON;
                    if (response && response.message) {
                        toastr.error(response.message); // Display the error message using toastr
                    } else {
                        toastr.error("حدث خطأ غير معروف."); // Fallback message
                    }
                }
            });
        }
    });
});

async function prepareInterview() {
    $.ajax({
        url: '/api/Customer/interview/start',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: async function (response) {
            if (response.success) {
                sessionData = response.data;
                console.log("Raw sessionData:", sessionData);
                const fetchedQuestions = sessionData.questions || sessionData.Questions || [];
                questions = Array.isArray(fetchedQuestions) ? fetchedQuestions : [];
                if (questions.length === 0) {
                    console.error("No questions received from API.");
                    isFailed = true;
                }
                isWaitingForApiResponse = false;
                updateStateDisplay();
                toggleButtons();
                questionNumDiv.textContent = currentQuestionIndex + 1;

                if (!isFailed) {
                    await startInterview();
                }
            } else {
                console.error('Unexpected response:', response);
                isWaitingForApiResponse = false;
                isFailed = true;
                updateStateDisplay();
            }
        },
        error: function (xhr, status, error) {
            console.error('Error starting the interview:', status, error);
            isWaitingForApiResponse = false;
            isFailed = true;
            updateStateDisplay();
        }
    });
}

async function submitAnswers(answers) {
    if (!answers || answers.length === 0) {
        console.error('No answers to submit:', answers);
        return;
    }

    const cleanedAnswers = answers
        .filter(answer => answer && typeof answer === 'object')
        .map(answer => ({
            Answer: typeof answer.Answer === 'string' && answer.Answer.trim() ? answer.Answer : "",
            TimeToken: typeof answer.TimeToken === 'number' && !isNaN(answer.TimeToken) && answer.TimeToken > 0 ? answer.TimeToken : 1
        }));

    if (cleanedAnswers.length === 0) {
        console.error('No valid answers to submit after cleaning:', answers);
        return;
    }

    console.log("Final cleaned answers before send:", JSON.stringify(cleanedAnswers, null, 2));
    try {
        await $.ajax({
            url: '/api/Customer/Interview/result',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(cleanedAnswers),
            success: function () {
                window.location.href = '/Customer/Interview/InterviewResult';
            },
            error: function (xhr, status, error) {
                const errorText = xhr.responseText || 'Unknown error';
                console.log("Server response details:", {
                    status: xhr.status,
                    statusText: xhr.statusText,
                    body: errorText
                });
                throw new Error(`Failed to submit answers: ${xhr.status} - ${errorText}`);
            }
        });
    } catch (error) {
        console.error('Error submitting answers:', error);
    }
}


// AJAX Call to Fetch Interview Session
$(document).ready(function () {
    questionNumDiv.textContent = "";
    questionTimer.textContent = "00:00";
    questionText.textContent = "";
    isWaitingForApiResponse = true;
    appState = "يفكر";
    bubble.classList.add("processing");
    updateStateDisplay();
    pauseInterviewBtn.disabled = true;

    resetInactivityTimer();
});

// Initial Setup
prepareInterviewBtn.addEventListener("click", () => {
    prepareInterviewBtn.style.display = "none";
    bubble.style.display = "flex";
    questionText.style.display = "block";
    stateDisplay.style.display = "flex";

    setupAudioAnalyzer();
    updateStateDisplay();
    toggleButtons();
    waitForVoices();
    pauseInterviewBtn.disabled = "none";

    prepareInterview();

});



// Inactivity timeout setup
let inactivityTimeout;

function resetInactivityTimer() {
    // Clear any existing timeout to reset the countdown
    clearTimeout(inactivityTimeout);
    // Set a new timeout to redirect after 10 minutes
    inactivityTimeout = setTimeout(() => {
        window.location.replace("/customer/Home/Index");
    }, 600000); // 10 minutes = 600,000 milliseconds
}

// Attach event listeners to detect user activity
document.addEventListener("mousemove", resetInactivityTimer);
document.addEventListener("mousedown", resetInactivityTimer);
document.addEventListener("mouseup", resetInactivityTimer);
document.addEventListener("click", resetInactivityTimer);
document.addEventListener("keydown", resetInactivityTimer);
document.addEventListener("keyup", resetInactivityTimer);
document.addEventListener("touchstart", resetInactivityTimer);
document.addEventListener("touchmove", resetInactivityTimer);
document.addEventListener("touchend", resetInactivityTimer);
window.addEventListener("scroll", resetInactivityTimer);

