// DOM Elements
const bubble = document.getElementById("interviewerAIBubble");
const stateDisplay = document.getElementById("interviewerAIState");
const repeatQuestionBtn = document.getElementById("repeatQuestion");
const startAnswerBtn = document.getElementById("startAnswer");
const endAnswerBtn = document.getElementById("endAnswer");
const pauseInterviewBtn = document.getElementById("pauseInterview");
const questionNumDiv = document.getElementById("questionNum");
const questionTimer = document.getElementById("questionTimer");

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
    if (appState === "يستمع") stateDisplay.textContent = "يستمع";
    else if (appState === "يفكر") stateDisplay.textContent = "يفكر";
    else if (appState === "يتكلم") stateDisplay.textContent = "يتكلم";
    else if (isPaused) stateDisplay.textContent = "متوقف";
    else stateDisplay.textContent = "جاهز"; // Default to "جاهز" when idle
    toggleButtons();
}

// Toggle Button States
function toggleButtons() {
    const isIdle = appState === "جاهز" || isPaused;
    const questionCount = Array.isArray(questions) ? questions.length : 0;
    repeatQuestionBtn.disabled = !isIdle || currentQuestionIndex >= questionCount || sessionData === null;
    startAnswerBtn.disabled = !isIdle || currentQuestionIndex >= questionCount || sessionData === null;
    endAnswerBtn.disabled = appState !== "يستمع";
    pauseInterviewBtn.textContent = isPaused ? "استكمال المقابلة" : "توقيف المقابلة";
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
        const maxScale = 1.5;
        const clampedScale = Math.min(Math.max(rawScale, minScale), maxScale);
        bubble.style.transform = `scale(${clampedScale})`;
        animationFrameId = requestAnimationFrame(step);
    }
    if (animationFrameId) cancelAnimationFrame(animationFrameId);
    animationFrameId = requestAnimationFrame(step);
}

// Speech Recognition Setup
const recognition = new (window.SpeechRecognition || window.webkitSpeechRecognition)();
recognition.lang = "ar-EG";
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
    if (isPaused) {
        console.log("Recognition stopped due to pause");
        return;
    }
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
    bubble.classList.add("processing"); // Thinking shape
    bubble.innerHTML = "";

    setTimeout(async () => {
        const text = accumulatedText.trim();
        answers[currentQuestionIndex] = {
            question: questions[currentQuestionIndex].originalQuestion,
            answer: text,
            timestamp: new Date().toISOString()
        };
        console.log("Stored answers:", answers);

        accumulatedText = "";
        repeatClickCount = 0;
        if (currentQuestionIndex < questions.length - 1) {
            currentQuestionIndex++;
            questionNumDiv.textContent = currentQuestionIndex + 1;
            updateStateDisplay();
            await proceedToNextQuestion();
        } else {
            await think(2000);
            if (isPaused) return;
            await speakText(sessionData?.conclusionText || "حسناً، سأقوم بتقييم مقابلتك الآن، شكراً لاستخدام موقع myInterview!");
            questionNumDiv.style.display = "none";
            questionTimer.style.display = "none";
        }
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
    timeLeft = timeLeft || 10; // Use saved timeLeft if resuming
    questionTimer.textContent = `${timeLeft} ثانية`;
    timer = setInterval(() => {
        timeLeft--;
        questionTimer.textContent = `${timeLeft} ثانية`;
        if (timeLeft <= 0) {
            clearInterval(timer);
            recognition.stop();
        }
    }, 1000);
}

// Display Countdown Timer (5, 4, 3, 2, 1)
function displayCountdownTimer() {
    return new Promise((resolve) => {
        let countdown = 5;
        appState = "جاهز";
        bubble.classList.remove("processing", "speaking", "listening");
        bubble.innerHTML = `<div class="countdown-number">${countdown}</div>`;
        updateStateDisplay();

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
                updateStateDisplay();
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
    arabicVoice = voices.find((voice) => voice.lang === "ar-SA") ||
        voices.find((voice) => voice.lang === "ar-EG") ||
        voices[0];
}

// Speak Text
function speakText(text) {
    if (!text) return Promise.resolve();
    return new Promise((resolve) => {
        const utterance = new SpeechSynthesisUtterance(text);
        utterance.lang = "ar-EG";
        if (arabicVoice) utterance.voice = arabicVoice;
        utterance.rate = 0.9;

        utterance.onstart = () => {
            appState = "يتكلم";
            updateStateDisplay();
            bubble.classList.remove("processing", "listening");
            bubble.classList.add("speaking"); // Speaking shape
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

        speechSynthesis.speak(utterance);
    });
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
        timeLeft = 10; // Reset timer for the new question
        questionTimer.textContent = `${timeLeft} ثانية`;
        const question = questions[currentQuestionIndex];
        const questionText = (question.linkingPhrase ? question.linkingPhrase + ", " : "") + question.originalQuestion;
        await speakText(questionText);
    }
}

// Start Interview Sequence
async function startInterview() {
    if (!sessionData) return;

    await displayCountdownTimer(); // Step 1: Countdown 5 to 1
    if (isPaused) return;
    await speakText(sessionData.introText || "مرحباً، شكراً لانضمامك إلى المقابلة."); // Step 2: Speak intro
    if (isPaused) return;
    await think(1000); // Step 3: Think for 1 second
    if (isPaused) return;
    timeLeft = 10; // Reset timer for the first question
    questionTimer.textContent = `${timeLeft} ثانية`;
    const firstQuestion = questions[0];
    const firstQuestionText = (firstQuestion.linkingPhrase ? firstQuestion.linkingPhrase + ", " : "") + firstQuestion.originalQuestion;
    questionNumDiv.textContent = 1;
    await speakText(firstQuestionText); // Step 4: Speak first question
}

// Button Handlers
repeatQuestionBtn.onclick = async () => {
    if (appState !== "جاهز" || currentQuestionIndex >= questions.length || sessionData === null) return;
    const question = questions[currentQuestionIndex];

    repeatClickCount++;
    if (repeatClickCount % 2 === 1) {
        await speakText(question.rephrasedQuestion || "لم يتم توفير سؤال معاد صياغته.");
    } else {
        await speakText(question.explanation || "لم يتم توفير تفسير لهذا السؤال.");
    }
};

startAnswerBtn.onclick = () => {
    if (appState !== "جاهز" || currentQuestionIndex >= questions.length || sessionData === null) return;
    accumulatedText = "";
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
    var interviewRequest = {
        applicantName: "جون",
        interviewerName: "محمد",
        topic: "Backend c#",
        department: "Programming",
        level: "Jenior",
        tone: "السورية",
        language: "الانجليزية",
        questionCount: 3
    };

    $.ajax({
        url: '/api/Customer/interview/start',
        type: 'POST',
        data: JSON.stringify(interviewRequest),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                sessionData = response.data;
                console.log("Raw sessionData:", sessionData);
                const fetchedQuestions = sessionData.questions || sessionData.Questions || [];
                questions = Array.isArray(fetchedQuestions) ? fetchedQuestions : [];
                updateStateDisplay();
                toggleButtons();
                questionNumDiv.textContent = currentQuestionIndex + 1;

                console.log("Session Data:", sessionData);
                console.log("Processed Questions:", questions);
            } else {
                console.error('Unexpected response:', response);
                updateStateDisplay();
            }
        },
        error: function (xhr, status, error) {
            console.error('Error starting the interview:', status, error);
            updateStateDisplay();
        }
    });
});

// Initial Setup and Automatic Start
window.addEventListener("load", async () => {
    setupAudioAnalyzer();
    appState = "جاهز";
    updateStateDisplay();
    toggleButtons();
    speechSynthesis.onvoiceschanged = loadArabicVoice;
    loadArabicVoice();

    // Automatically start the interview after page load
    await startInterview();
});