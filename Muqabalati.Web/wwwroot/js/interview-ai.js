
// DOM Elements
const bubble = document.getElementById("interviewerAIBubble");
const stateDisplay = document.getElementById("interviewerAIState");
const startAnswerBtn = document.getElementById("startAnswer");
const endAnswerBtn = document.getElementById("endAnswer");
const questionTimer = document.getElementById("questionTimer");
const textArea = document.getElementById("textTest");
const playQuestionBtn = document.getElementById("playQuestion");
const repeatQuestionBtn = document.getElementById("repeatQuestion");
const showAnswerTextBtn = document.getElementById("showAnswerText");
const questionTextDiv = document.getElementById("questionText");
const questionNumDiv = document.getElementById("questionNum");

// Variables
let audioContext, analyzer, source, dataArray;
let appState = "idle"; // "idle", "listening", "thinking", "speaking"
let timer;
let timeLeft = 10;
let accumulatedText = "";
let currentQuestionIndex = 0;
let answerText = "";
let arabicVoice = null;
let animationFrameId = null;

// Questions List
const questions = [
    "ما هو الغرض من الكلمتين async و await في C#؟",
    "ما هو LINQ وكيف يُستخدم في C#؟",
    "صف نمط MVC ومكوناته.",
];

// Setup Audio Analyzer
function setupAudioAnalyzer() {
    if (!audioContext) {
        audioContext = new (window.AudioContext ||
            window.webkitAudioContext)();
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
    if (appState === "listening") stateDisplay.textContent = "يستمع";
    else if (appState === "thinking") stateDisplay.textContent = "يفكر";
    else if (appState === "speaking") stateDisplay.textContent = "يتكلم";
    else stateDisplay.textContent = "جاهز";
    toggleButtons();
}

// Toggle Button States
function toggleButtons() {
    const isIdle = appState === "idle";
    playQuestionBtn.disabled =
        !isIdle || currentQuestionIndex >= questions.length;
    repeatQuestionBtn.disabled =
        !isIdle || currentQuestionIndex >= questions.length;
    startAnswerBtn.disabled =
        !isIdle || currentQuestionIndex >= questions.length;
    endAnswerBtn.disabled = appState !== "listening";
    showAnswerTextBtn.disabled = !isIdle || !answerText;
    questionTextDiv.classList.toggle(
        "disabled",
        currentQuestionIndex >= questions.length
    );
    questionTextDiv.style.pointerEvents =
        currentQuestionIndex < questions.length ? "auto" : "none";
}

// Animate Bubble During Listening
function animateListeningBubble() {
    function step() {
        if (appState !== "listening" || !source) {
            bubble.style.transform = "scale(1)";
            animationFrameId = requestAnimationFrame(step);
            return;
        }
        analyzer.getByteFrequencyData(dataArray);
        const average =
            dataArray.reduce((acc, val) => acc + val, 0) / dataArray.length;
        const scale = average > 10 ? 1 + average / 150 : 1;
        bubble.style.transform = `scale(${Math.max(1, scale)})`;
        animationFrameId = requestAnimationFrame(step);
    }
    if (animationFrameId) cancelAnimationFrame(animationFrameId);
    animationFrameId = requestAnimationFrame(step);
}

// Speech Recognition Setup
const recognition = new (window.SpeechRecognition ||
    window.webkitSpeechRecognition)();
recognition.lang = "ar-EG";
recognition.continuous = true;
recognition.interimResults = false;

recognition.onstart = () => {
    appState = "listening";
    updateStateDisplay();
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
    const result = event.results[event.results.length - 1][0].transcript;
    accumulatedText += result + " ";
};

recognition.onend = () => {
    appState = "thinking";
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
    bubble.classList.remove("speaking");
    bubble.classList.add("processing");
    bubble.innerHTML = "";

    setTimeout(() => {
        const text = accumulatedText.trim();
        answerText = text;
        textArea.value = text;
        accumulatedText = "";

        if (text) {
            speakText("حسنا، لقد سمعتك");
            setTimeout(() => {
                if (currentQuestionIndex < questions.length - 1) {
                    currentQuestionIndex++;
                    questionNumDiv.textContent = currentQuestionIndex + 1;
                    playNextQuestion();
                } else {
                    speakText(
                        "حسنا، سأقوم بتقييم مقابلتك الآن، شكرا لاستخدام موقع myInteerview!"
                    );
                }
            }, 2000);
        } else {
            appState = "idle";
            updateStateDisplay();
            bubble.classList.remove("processing");
        }
    }, 2000);
};

recognition.onerror = (event) => {
    stateDisplay.textContent = "خطأ: " + event.error;
    appState = "idle";
    updateStateDisplay();
    bubble.classList.remove("processing");
    if (animationFrameId) {
        cancelAnimationFrame(animationFrameId);
        animationFrameId = null;
    }
};

// Start Timer
function startTimer() {
    timeLeft = 10;
    questionTimer.textContent = "10s";
    timer = setInterval(() => {
        timeLeft--;
        questionTimer.textContent = `${timeLeft}s`;
        if (timeLeft <= 0) {
            clearInterval(timer);
            recognition.stop();
        }
    }, 1000);
}

// Load Arabic Voice
function loadArabicVoice() {
    const voices = speechSynthesis.getVoices();
    arabicVoice =
        voices.find((voice) => voice.lang === "ar-SA") ||
        voices.find((voice) => voice.lang === "ar-EG") ||
        voices[0];
}

// Speak Text
function speakText(text) {
    if (!text) return;
    const utterance = new SpeechSynthesisUtterance(text);
    utterance.lang = "ar-EG";
    if (arabicVoice) utterance.voice = arabicVoice;
    utterance.rate = 0.9;

    utterance.onstart = () => {
        appState = "speaking";
        updateStateDisplay();
        bubble.classList.remove("processing");
        bubble.classList.add("speaking");
        bubble.innerHTML =
            '<div class="dot"></div><div class="dot"></div><div class="dot"></div>';
    };

    utterance.onend = () => {
        appState = "idle";
        updateStateDisplay();
        bubble.classList.remove("speaking");
        bubble.innerHTML = "";
    };

    speechSynthesis.speak(utterance);
}

// Play Next Question
function playNextQuestion() {
    if (currentQuestionIndex < questions.length) {
        const question = questions[currentQuestionIndex];
        appState = "thinking";
        updateStateDisplay();
        bubble.classList.add("processing");
        setTimeout(() => {
            speakText(question);
        }, 2000);
    }
}

// Button Handlers
playQuestionBtn.onclick = () => {
    if (appState !== "idle" || currentQuestionIndex >= questions.length)
        return;
    playNextQuestion();
};

repeatQuestionBtn.onclick = () => {
    if (appState !== "idle" || currentQuestionIndex >= questions.length)
        return;
    speakText(questions[currentQuestionIndex]);
};

startAnswerBtn.onclick = () => {
    if (appState !== "idle" || currentQuestionIndex >= questions.length)
        return;
    accumulatedText = "";
    startTimer();
    recognition.start();
};

endAnswerBtn.onclick = () => {
    if (appState !== "listening") return;
    clearInterval(timer);
    recognition.stop();
};

questionTextDiv.onclick = () => {
    if (currentQuestionIndex < questions.length) {
        document.getElementById("questionModalBody").textContent =
            questions[currentQuestionIndex];
        const questionModal = new bootstrap.Modal(
            document.getElementById("questionModal")
        );
        questionModal.show();
    }
};

showAnswerTextBtn.onclick = () => {
    if (answerText) {
        document.getElementById("answerTextModalBody").textContent =
            answerText;
        const answerTextModal = new bootstrap.Modal(
            document.getElementById("answerTextModal")
        );
        answerTextModal.show();
    }
};

// Initial Setup
window.addEventListener("load", () => {
    setupAudioAnalyzer();
    appState = "idle";
    updateStateDisplay();
    toggleButtons();
    speechSynthesis.onvoiceschanged = loadArabicVoice;
    loadArabicVoice();
});