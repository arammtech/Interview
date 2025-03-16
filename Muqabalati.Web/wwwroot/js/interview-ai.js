// DOM Elements
const bubble = document.getElementById("interviewerAIBubble");
const stateDisplay = document.getElementById("interviewerAIState");
const repeatQuestionBtn = document.getElementById("repeatQuestion");
const startAnswerBtn = document.getElementById("startAnswer");
const endAnswerBtn = document.getElementById("endAnswer");
const skipQuestionBtn = document.getElementById("skipQuestion");
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
let isWaitingForApiResponse = true; // Track API response state
let isFailed = false; // If the request failed state
let isReady = false; // 
let isEvaluating = false;
let answerStartTime = null;
let isProcessingEnd = false;
let voiceGender = "female"; // Default to male, can be changed to "female"
let accent = "ar-EG"; // Default to Saudi Arabic, modifiable for other accents
let voicesLoadedPromise = null;

// Removed Variables: isPaused, pauseStartTime, savedState

// Speech Recognition Setup
const recognition = new (window.SpeechRecognition || window.webkitSpeechRecognition)();
recognition.lang = "ar-SA"; // Arabic language
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
    } else if (isReady) {
        stateDisplay.textContent = "بدء المقابلة";
        isReady = false;
    } else if (appState === "جاهز" && currentQuestionIndex < questions.length) {
        stateDisplay.textContent = "اضغط لبدء الإجابة";
        bubble.style.transform = "scale(1)"; // Explicitly reset scale
        bubble.classList.remove("speaking", "processing", "listening"); // Ensure no classes interfere
    } else {
        stateDisplay.textContent = "جاهز";
        bubble.style.transform = "scale(1)"; // Explicitly reset scale
        bubble.classList.remove("speaking", "processing", "listening");
    }
    toggleButtons(); // Call toggleButtons to update button states
}
// Toggle Button States
function toggleButtons() {
    const isIdle = appState === "جاهز";
    const questionCount = Array.isArray(questions) ? questions.length : 0;

    // Update 2: Disable pause button during evaluation and preparation
    const canPause = (appState === "يتكلم" || appState === "يفكر" || appState === "يستمع" || isIdle) && !isEvaluating && !isWaitingForApiResponse;

    // Helper function to apply button states with a delay
    const applyButtonStates = () => {
        if (isPaused) {
            // When paused, disable all buttons except pause (resume) button
            repeatQuestionBtn.disabled = true;
            startAnswerBtn.disabled = true;
            skipQuestionBtn.disabled = true;
            endAnswerBtn.disabled = appState !== "يستمع"; // Keep enabled if listening (from previous logic)
            pauseInterviewBtn.disabled = false;

            // Update 3: Add visual feedback
            repeatQuestionBtn.classList.add("disabled-button");
            startAnswerBtn.classList.add("disabled-button");
            skipQuestionBtn.classList.add("disabled-button");
            endAnswerBtn.classList.toggle("disabled-button", appState !== "يستمع");
            pauseInterviewBtn.classList.remove("disabled-button");
        } else {
            // When not paused, enable buttons based on state
            // Update 1: Enable repeatQuestionBtn during listening state
            repeatQuestionBtn.disabled = !(isIdle || appState === "يستمع") || currentQuestionIndex >= questionCount || sessionData === null;
            startAnswerBtn.disabled = !isIdle || currentQuestionIndex >= questionCount || sessionData === null;
            endAnswerBtn.disabled = appState !== "يستمع";
            skipQuestionBtn.disabled = !isIdle || currentQuestionIndex >= questionCount || sessionData === null;
            pauseInterviewBtn.disabled = !canPause;

            // Update 3: Add visual feedback
            repeatQuestionBtn.classList.toggle("disabled-button", !(isIdle || appState === "يستمع") || currentQuestionIndex >= questionCount || sessionData === null);
            startAnswerBtn.classList.toggle("disabled-button", !isIdle || currentQuestionIndex >= questionCount || sessionData === null);
            endAnswerBtn.classList.toggle("disabled-button", appState !== "يستمع");
            skipQuestionBtn.classList.toggle("disabled-button", !isIdle || currentQuestionIndex >= questionCount || sessionData === null);
            pauseInterviewBtn.classList.toggle("disabled-button", !canPause);
        }

    // Remove pause/resume SVG logic since pause is disabled
    pauseInterviewBtn.disabled = true; // Disable the pause button
    pauseInterviewBtn.innerHTML = ""; // Clear the button content
}

// Animate Listening Bubble
function animateListeningBubble() {
    function step() {
        if (appState !== "يستمع" || !source) {
            bubble.style.transform = "scale(1)";
            bubble.classList.remove("listening");
            cancelAnimationFrame(animationFrameId); // Stop the loop
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
            animateListeningBubble(); // Listening shape
        })
        .catch((err) => console.error("Audio stream error:", err));
};

recognition.onresult = (event) => {
    const result = event.results[event.results.length - 1][0].transcript;
    accumulatedText += result + " ";
};

recognition.onend = () => {
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
        Answer: text || "No response provided",
        TimeToken: isNaN(timeTaken) ? 0 : timeTaken
    };
    console.log("Stored answer at index", currentQuestionIndex, ":", answers[currentQuestionIndex]);

    accumulatedText = "";
    repeatClickCount = 0;

    (async () => {
        if (currentQuestionIndex < questions.length - 1) {
            currentQuestionIndex++;
            questionNumDiv.textContent = `${currentQuestionIndex + 1}/${questions.length}`;
            await think(2000); // Keep this for transitions between questions
            const question = questions[currentQuestionIndex];
            const estimatedTimeSeconds = question.estimatedTimeMinutes * 60;
            questionTimer.textContent = formatTime(estimatedTimeSeconds);
            const questionTextContent = (question.linkingPhrase ? question.linkingPhrase + ", " : "") + question.originalQuestion;
            questionText.textContent = questionTextContent;
            await speakText(questionTextContent);
        } else {
            // New logic for last question
            questionNumDiv.style.display = "none";
            questionTimer.textContent = "00:00";
            questionText.textContent = "";
            const conclusionText = sessionData?.conclusionText || "حسناً، سأقوم بتقييم مقابلتك الآن، شكراً لاستخدام موقع مقابلتي!";
            await speakText(conclusionText);

            // Enter persistent processing state
            appState = "يفكر";
            isEvaluating = true;
            bubble.classList.remove("speaking", "listening");
            bubble.classList.add("processing"); // Infinite processing animation
            updateStateDisplay(); // Display "جاري تقييم النتيجة"

            // Wait for server response and redirect
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

// Set default timer display
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
            appState = "جاهز";
            bubble.style.transform = "scale(1)"; // Reset scale after speaking
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
    const introText = sessionData?.introText || "مرحباً، شكراً لانضمامك إلى المقابلة.";
    await speakText(introText);
    await think(1000);

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

skipQuestionBtn.onclick = () => {
    if (appState !== "جاهز" || currentQuestionIndex >= questions.length || sessionData === null) return;

    // Fill the answer with an empty string (skip)
    const answerEndTime = new Date().getTime();
    const timeTaken = answerStartTime ? (answerEndTime - answerStartTime) / 1000 : 0;
    answers[currentQuestionIndex] = {
        Answer: "",
        TimeToken: isNaN(timeTaken) ? 0 : timeTaken
    };
    console.log("Skipped answer at index", currentQuestionIndex, ":", answers[currentQuestionIndex]);

    // Mimic the end of the answer (similar to recognition.onend)
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
            await think(2000); // Keep this for transitions between questions
            const question = questions[currentQuestionIndex];
            const estimatedTimeSeconds = question.estimatedTimeMinutes * 60;
            questionTimer.textContent = formatTime(estimatedTimeSeconds);
            const questionTextContent = (question.linkingPhrase ? question.linkingPhrase + ", " : "") + question.originalQuestion;
            questionText.textContent = questionTextContent;
            await speakText(questionTextContent);
        } else {
            // New logic for last question
            questionNumDiv.style.display = "none";
            questionTimer.textContent = "00:00";
            questionText.textContent = "";
            const conclusionText = sessionData?.conclusionText || "حسناً، سأقوم بتقييم مقابلتك الآن، شكراً لاستخدام موقع مقابلتي!";
            await speakText(conclusionText);

            // Enter persistent processing state
            appState = "يفكر";
            isEvaluating = true;
            bubble.classList.remove("speaking", "listening");
            bubble.classList.add("processing"); // Infinite processing animation
            updateStateDisplay(); // Display "جاري تقييم النتيجة"

            // Wait for server response and redirect
            await submitAnswers(answers);
        }
        answerStartTime = null;
        isProcessingEnd = false;
    })();
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

async function submitAnswers(answers) {
    if (!answers || answers.length === 0) {
        console.error('No answers to submit:', answers);
        return;
    }

    const cleanedAnswers = answers
        .filter(answer => answer && typeof answer === 'object')
        .map(answer => ({
            Answer: typeof answer.Answer === 'string' && answer.Answer.trim() ? answer.Answer : "No response provided",
            TimeToken: typeof answer.TimeToken === 'number' && !isNaN(answer.TimeToken) && answer.TimeToken > 0 ? answer.TimeToken : 1
        }));

    if (cleanedAnswers.length === 0) {
        console.error('No valid answers to submit after cleaning:', answers);
        return;
    }

    console.log("Final cleaned answers before send:", JSON.stringify(cleanedAnswers, null, 2));
    try {
        await $.ajax({
            url: '/Customer/Interview/Result',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(cleanedAnswers),
            success: function (data) {
                console.log("Submission successful, server will redirect:", data);
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

// Initial Setup
window.addEventListener("load", () => {
    setupAudioAnalyzer();
    updateStateDisplay();
    toggleButtons();
    waitForVoices(); // Replace direct loadArabicVoice call
});