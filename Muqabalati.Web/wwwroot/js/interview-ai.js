// DOM Elements
const bubble = document.getElementById("interviewerAIBubble");
const stateDisplay = document.getElementById("interviewerAIState");
const repeatQuestionBtn = document.getElementById("repeatQuestion");
const startAnswerBtn = document.getElementById("startAnswer");
const endAnswerBtn = document.getElementById("endAnswer");
const pauseInterviewBtn = document.getElementById("pauseInterview");
const questionNumDiv = document.getElementById("questionNum");
const questionTimer = document.getElementById("questionTimer");
const questionText = document.getElementById("questionText");

// Variables 
let audioContext, analyzer, source, dataArray;
let appState = "جاهز"; // "جاهز" (ready), "يتكلم" (speaking), "يفكر" (thinking), "يستمع" (listening)
let timer;
let timeLeft = 10;
let accumulatedText = "";
let currentQuestionIndex = 0;
let arabicVoice = null;
let animationFrameId = null;
let sessionData = null; // Stores API response
let questions = []; // Populated from API
let answers = []; // Stores user answers
let repeatClickCount = 0; // Tracks "Repeat Question" clicks
let isPaused = false; // Pause status
let pauseStartTime = null; // Tracks pause start time 
let savedState = null; // Saves state during pause
let isWaitingForApiResponse = true; // Track API response state
let isFailed = false; // If the request failed state
let isReady = false; // 
let isEvaluating = false;
let answerStartTime = null;
let isProcessingEnd = false; 
let voiceGender = "female"; // Default to male, can be changed to "female"
let accent = "ar-EG"; // Default to Saudi Arabic, modifiable for other accents
let voicesLoadedPromise = null;

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
    if (isFailed) {
        stateDisplay.textContent = "فشل في بدء المقابلة";
    } else if (appState === "يستمع") {
        stateDisplay.textContent = "يستمع";
    } else if (appState === "يفكر") {
        if (isEvaluating) {
            stateDisplay.textContent = "جاري تقييم النتيجة"; // Show during evaluation
        } else if (isWaitingForApiResponse) {
            stateDisplay.textContent = "جاري تجهيز المقابلة";
        } else if (currentQuestionIndex >= questions.length) {
            stateDisplay.textContent = "جاري تقييم مقابلتك...";
        } else {
            stateDisplay.textContent = "يفكر";
        }
    } else if (appState === "يتكلم") {
        stateDisplay.textContent = "يتكلم";
    } else if (isPaused) {
        stateDisplay.textContent = "متوقف";
    } else if (isReady) {
        stateDisplay.textContent = "بدء المقابلة";
        isReady = false;
    } else if (appState === "جاهز" && currentQuestionIndex < questions.length) {
        stateDisplay.textContent = "اضغط لبدء الإجابة";
    } else {
        stateDisplay.textContent = "جاهز";
    }
    toggleButtons();
}
// Toggle Button States
// Toggle Button States
function toggleButtons() {
    const isIdle = appState === "جاهز" || isPaused;
    const questionCount = Array.isArray(questions) ? questions.length : 0;
    repeatQuestionBtn.disabled = !isIdle || currentQuestionIndex >= questionCount || sessionData === null;
    startAnswerBtn.disabled = !isIdle || currentQuestionIndex >= questionCount || sessionData === null;
    endAnswerBtn.disabled = appState !== "يستمع";

    // SVG for Pause
    const pauseSvg = `<svg width="25" height="24" viewBox="0 0 25 24" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M9.14 21.75H5.51C3.65 21.75 2.75 20.89 2.75 19.11V4.89C2.75 3.11 3.65 2.25 5.51 2.25H9.14C11 2.25 11.9 3.11 11.9 4.89V19.11C11.9 20.89 11 21.75 9.14 21.75ZM5.51 3.75C4.43 3.75 4.25 4.02 4.25 4.89V19.11C4.25 19.98 4.42 20.25 5.51 20.25H9.14C10.22 20.25 10.4 19.98 10.4 19.11V4.89C10.4 4.02 10.23 3.75 9.14 3.75H5.51Z" fill="#F4F4F4"/>
        <path d="M19.4901 21.75H15.8601C14.0001 21.75 13.1001 20.89 13.1001 19.11V4.89C13.1001 3.11 14.0001 2.25 15.8601 2.25H19.4901C21.3501 2.25 22.2501 3.11 22.2501 4.89V19.11C22.2501 20.89 21.3501 21.75 19.4901 21.75ZM15.8601 3.75C14.7801 3.75 14.6001 4.02 14.6001 4.89V19.11C14.6001 19.98 14.7701 20.25 15.8601 20.25H19.4901C20.5701 20.25 20.7501 19.98 20.7501 19.11V4.89C20.7501 4.02 20.5801 3.75 19.4901 3.75H15.8601Z" fill="#F4F4F4"/>
    </svg>`;

    // SVG for Resume
    const resumeSvg = `<svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M4 11.9999V8.43989C4 4.01989 7.13 2.2099 10.96 4.4199L14.05 6.1999L17.14 7.9799C20.97 10.1899 20.97 13.8099 17.14 16.0199L14.05 17.7999L10.96 19.5799C7.13 21.7899 4 19.9799 4 15.5599V11.9999Z" stroke="#F4F4F4" stroke-width="1.5" stroke-miterlimit="10" stroke-linecap="round" stroke-linejoin="round"/>
    </svg>`;

    // Update the pause/resume button with SVG
    pauseInterviewBtn.innerHTML = isPaused ? resumeSvg : pauseSvg;
}

// Animate Listening Bubble
function animateListeningBubble() {
    function step() {
        if (appState !== "يستمع" || !source) {
            bubble.style.transform = "scale(1)";
            bubble.classList.remove("listening");
            animationFrameId = requestAnimationFrame(step);
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
    // Start with minScale (1) when entering listening state
    bubble.style.transform = "scale(1)";
    bubble.classList.add("listening");
    animationFrameId = requestAnimationFrame(step);
}

// Speech Recognition Setup
const recognition = new (window.SpeechRecognition || window.webkitSpeechRecognition)();
recognition.lang = "ar"; // change this for accent
recognition.continuous = true;
recognition.interimResults = false;

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
            animateListeningBubble(); // Listening shape
        })
        .catch((err) => console.error("Audio stream error:", err));
};

recognition.onresult = (event) => {
    const result = event.results[event.results.length - 1][0].transcript;
    accumulatedText += result + " ";
};


recognition.onend = () => {
    if (isPaused || isProcessingEnd) {
        console.log("Recognition stopped due to pause or already processing");
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

    setTimeout(async () => {
        const text = accumulatedText.trim();
        answers[currentQuestionIndex] = {
            Answer: text,
            TimeToken: timeTaken
        };
        console.log("Stored answers:", answers);

        accumulatedText = "";
        repeatClickCount = 0;
        if (currentQuestionIndex < questions.length - 1) {
            currentQuestionIndex++;
            questionNumDiv.textContent = `${currentQuestionIndex + 1}/${questions.length}`;
            await think(2000);
            if (isPaused) return;
            const question = questions[currentQuestionIndex];
            const estimatedTimeSeconds = question.estimatedTimeMinutes * 60;
            questionTimer.textContent = formatTime(estimatedTimeSeconds);
            const questionTextContent = (question.linkingPhrase ? question.linkingPhrase + ", " : "") + question.originalQuestion;
            questionText.textContent = questionTextContent;
            await speakText(questionTextContent);
        } else {
            questionNumDiv.style.display = "none";
            questionTimer.textContent = "00:00";
            questionText.textContent = "";
            await think(2000);
            if (isPaused) return;
            const conclusionText = sessionData?.conclusionText || "حسناً، سأقوم بتقييم مقابلتك الآن، شكراً لاستخدام موقع مقابلتي!";
            await speakText(conclusionText);
            if (isPaused) return;
            isEvaluating = true;
            await think(3000);
            isEvaluating = false;
            if (isPaused) return;
            console.log("Final answers:", answers);

            submitAnswers(answers);

            bubble.classList.remove("processing", "speaking", "listening");
            bubble.style.transform = "scale(1)";
            bubble.innerHTML = "";
            questionNumDiv.style.display = "none";
            questionTimer.style.display = "none";
        }
        answerStartTime = null;
        isProcessingEnd = false;
    }, 2000);
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
function startTimer() {
    const question = questions[currentQuestionIndex];
    const estimatedTimeMinutes = question.estimatedTimeMinutes || 1; // Default to 1 minute if missing
    timeLeft = estimatedTimeMinutes * 60; // Set timeLeft in seconds
    questionTimer.textContent = formatTime(timeLeft);

    timer = setInterval(() => {
        timeLeft--;
        questionTimer.textContent = formatTime(timeLeft);
        if (timeLeft <= 0) {
            clearInterval(timer);
            recognition.stop();
        }
    }, 1000);
}

// Helper function to format time in MM:SS
function formatTime(seconds) {
    const minutes = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${minutes.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
}

// Set default timer display (call this when initializing or waiting for a response)
questionTimer.textContent = "00:00";

// Display Countdown Timer (5, 4, 3, 2, 1)
function displayCountdownTimer() {
    return new Promise((resolve) => {
        let countdown = 5;
        appState = "جاهز";
        bubble.classList.remove("processing", "speaking", "listening");
        bubble.innerHTML = `<div class="countdown-number">${countdown}</div>`;
        stateDisplay.textContent = "بدء المقابلة"; // Set and keep this during countdown
        isReady = true;
        toggleButtons();

        const countdownInterval = setInterval(() => {
            countdown--;
            if (isPaused) {
                clearInterval(countdownInterval);
                return;
            }
            bubble.innerHTML = `<div class="countdown-number">${countdown}</div>`;
            if (countdown <= 0) {
                clearInterval(countdownInterval);
                bubble.innerHTML = "";
                appState = "جاهز";
                updateStateDisplay(); // Update only after countdown ends
                setTimeout(() => {
                    resolve();
                }, 100);
            }
        }, 1000);
    });
}

// Load Arabic Voice
function loadArabicVoice() {
    const voices = speechSynthesis.getVoices();
    arabicVoice = voices.find(voice =>
        voice.lang === accent &&
        voice.name.toLowerCase().includes(voiceGender === "male" ? "male" : "female")
    ) || voices.find(voice => voice.lang === accent) ||
        voices.find(voice => voice.lang.startsWith("ar")) ||
        (voices.length > 0 ? voices[0] : null);

    if (arabicVoice) {
        console.log(`Loaded voice: ${arabicVoice.name}, lang: ${arabicVoice.lang}, gender hint: ${voiceGender}`);
    } else {
        console.error("No voices available for speech synthesis.");
    }
}




// Speak Text with Customization
function speakText(text) {
    if (!text) return Promise.resolve();
    return new Promise(async (resolve) => {
        if (!arabicVoice) {
            console.warn("Arabic voice not loaded yet, waiting...");
            await waitForVoices();
            if (!arabicVoice) {
                console.error("No voice available to speak text:", text);
                resolve();
                return;
            }
        }

        const cleanText = text.replace(/[.,?\n]/g, "").trim();
        const words = cleanText.split(" ");
        let utteranceText = "";
        for (let word of words) {
            const isEnglish = /[a-zA-Z]/.test(word);
            if (isEnglish) {
                utteranceText += word + " ";
            } else {
                utteranceText += word + " ";
            }
        }
        utteranceText = utteranceText.trim();

        const voices = speechSynthesis.getVoices();
        let selectedVoice = voices.find(voice =>
            voice.lang === accent &&
            voice.name.toLowerCase().includes(voiceGender === "male" ? "male" : "female")
        ) || voices.find(voice => voice.lang === accent) || arabicVoice;

        if (!selectedVoice) {
            console.warn(`No ${voiceGender} voice found for ${accent}. Falling back to loaded Arabic voice: ${arabicVoice?.name}`);
            selectedVoice = arabicVoice;
        }

        const utterance = new SpeechSynthesisUtterance(utteranceText);
        utterance.lang = accent;
        utterance.voice = selectedVoice;
        utterance.volume = 1.0;
        utterance.rate = 0.8;
        utterance.pitch = voiceGender === "female" ? 1.2 : 0.9; // Adjust pitch for gender

        utterance.onstart = () => {
            appState = "يتكلم";
            updateStateDisplay();
            bubble.classList.remove("processing", "listening");
            bubble.classList.add("speaking");
            bubble.innerHTML = '<div class="dot"></div><div class="dot"></div><div class="dot"></div>';
        };

        utterance.onend = () => {
            if (isPaused) {
                console.log("Speaking stopped due to pause");
                return;
            }
            appState = "جاهز";
            updateStateDisplay();
            bubble.classList.remove("speaking", "processing", "listening");
            bubble.innerHTML = "";
            resolve();
        };

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
                    loadArabicVoice();
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
        bubble.classList.add("processing"); // Thinking shape
        const startTime = Date.now();
        function checkTime() {
            if (isPaused) {
                console.log("Thinking paused");
                return;
            }
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
        await think(2000); // Think for 2 seconds
        if (isPaused) return;

        const question = questions[currentQuestionIndex];
        const estimatedTimeSeconds = question.estimatedTimeMinutes * 60; // Convert minutes to seconds
        questionTimer.textContent = formatTime(estimatedTimeSeconds); // Display initial time in MM:SS
        const questionTextContent = (question.linkingPhrase ? question.linkingPhrase + ", " : "") + question.originalQuestion;
        questionText.textContent = questionTextContent;
        await speakText(questionTextContent);
    }
}
 
async function startInterview() {
    if (!sessionData) return;

    await waitForVoices(); // Ensure voices are loaded
    await displayCountdownTimer(); // Assuming this shows a 5,4,3,2,1 countdown
    if (isPaused) return;

    const introText = sessionData?.candidates || "مرحباً، شكراً لانضمامك إلى المقابلة.";
    await speakText(introText);
    if (isPaused) return;
     
    await think(1000);
    if (isPaused) return;

    const firstQuestion = questions[0];
    const firstQuestionText = (firstQuestion.linkingPhrase ? firstQuestion.linkingPhrase + ", " : "") + firstQuestion.originalQuestion;
    questionNumDiv.textContent = `1/${questions.length}`; // Set question counter
    questionText.textContent = firstQuestionText;
    await speakText(firstQuestionText);

    const estimatedTimeSeconds = firstQuestion.estimatedTimeMinutes * 60;
    questionTimer.textContent = formatTime(estimatedTimeSeconds); // Show estimated time
}

// Button Handlers
repeatQuestionBtn.onclick = async () => {
    if (appState !== "جاهز" || currentQuestionIndex >= questions.length || sessionData === null) return;
    const question = questions[currentQuestionIndex];

    repeatClickCount++;
    if (repeatClickCount % 2 === 1) {
        const rephrased = question.rephrasedQuestion || "لم يتم توفير سؤال معاد صياغته.";
        questionText.textContent = rephrased; // Update question text to rephrased
        await speakText(rephrased);
    } else {
        const explanation = question.explanation || "لم يتم توفير تفسير لهذا السؤال.";
        questionText.textContent = explanation; // Update question text to explanation
        await speakText(explanation);
    }
};

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

// Pause Interview Handler
pauseInterviewBtn.onclick = () => {
    if (!isPaused) {
        isPaused = true;
        pauseStartTime = new Date();
        savedState = {
            appState,
            currentQuestionIndex,
            timeLeft,
            accumulatedText,
            repeatClickCount,
            answers: [...answers]
        };
        console.log("Pausing interview:", savedState);

        // Stop all actions based on current state and set to idle
        if (appState === "يستمع") {
            clearInterval(timer);
            recognition.stop();
        } else if (appState === "يتكلم") {
            speechSynthesis.cancel();
        } else if (appState === "يفكر") {
            bubble.classList.remove("processing");
        }

        // Reset bubble to idle state (non-moving)
        bubble.classList.remove("speaking", "processing", "listening");
        bubble.style.transform = "scale(1)"; // Stop any scaling animation
        if (animationFrameId) {
            cancelAnimationFrame(animationFrameId);
            animationFrameId = null;
        }
        appState = "جاهز"; // Force idle state
        updateStateDisplay();
    } else {
        const pauseEndTime = new Date();
        const pauseDuration = (pauseEndTime - pauseStartTime) / 1000 / 60; // In minutes
        if (pauseDuration > 20) {
            console.log("Pause duration exceeded 20 minutes, redirecting...");
            window.location.href = "/Customer/Home/Index";
            return;
        }

        appState = savedState.appState;
        currentQuestionIndex = savedState.currentQuestionIndex;
        timeLeft = savedState.timeLeft;
        accumulatedText = savedState.accumulatedText;
        repeatClickCount = savedState.repeatClickCount;
        answers = [...savedState.answers];
        savedState = null;
        isPaused = false;
        console.log("Resuming interview:", { appState, currentQuestionIndex, timeLeft, accumulatedText });

        // Do not trigger any AI actions; just re-enable buttons
        updateStateDisplay();
    }
};

// AJAX Call to Fetch Interview Session
$(document).ready(function () {
    // Initial data
    questionNumDiv.textContent = ""; 
    questionTimer.textContent = "00:00";
    questionText.textContent = "";
    isWaitingForApiResponse = true;
    appState = "يفكر";
    bubble.classList.add("processing");
    updateStateDisplay();

    var interviewRequest = {
        applicantName: "جون",
        interviewerName: "محمد",
        topic: "Backend c#",
        department: "Programming",
        skillLevel: "Jenior",
        tone: "السورية",
        terminologyLanguage: "الانجليزية",
        questionCount: 3,
        interviewLanguage: "العربية"
    };

    $.ajax({
        url: '/api/Customer/interview/start',
        type: 'POST',
        data: JSON.stringify(interviewRequest),
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
                    isFailed = true; // Set failure if no questions
                }
                isWaitingForApiResponse = false;
                updateStateDisplay();
                toggleButtons();
                questionNumDiv.textContent = currentQuestionIndex + 1;

                if (!isFailed) {
                    await startInterview(); // Proceed only if not failed
                }
            } else {
                console.error('Unexpected response:', response);
                isWaitingForApiResponse = false;
                isFailed = true; // Set failure on unexpected response
                updateStateDisplay();
            }
        },
        error: function (xhr, status, error) {
            console.error('Error starting the interview:', status, error);
            isWaitingForApiResponse = false;
            isFailed = true; // Set failure on error
            updateStateDisplay();
        }
    });
});
//Explanation:

async function submitAnswers(answers) {
    if (!answers || answers.length === 0) {
        console.error('No answers to submit:', answers);
        return; // Prevent sending empty or undefined data
    }
    try {
        console.log("Sending answers:", answers); // Debug log to verify data
        const response = await fetch('/Customer/Interview/Result', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(answers)
        });

        if (!response.ok) {
            const errorText = await response.text(); // Get detailed error message
            throw new Error(`Failed to submit answers: ${response.status} - ${errorText}`);
        }
        console.log("sent! answers");
        // No redirect needed; server will handle view rendering
    } catch (error) {
        console.error('Error submitting answers:', error);
    }
}

// Initial Setup
window.addEventListener("load", () => {
    setupAudioAnalyzer();
    updateStateDisplay();
    toggleButtons();
    waitForVoices(); // Replace direct loadArabicVoice call
});