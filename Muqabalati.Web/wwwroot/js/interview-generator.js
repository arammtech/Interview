document.addEventListener('DOMContentLoaded', () => {
    // Get DOM elements
    const topicInput = document.getElementById('inputTopic');
    const charCountDisplay = document.querySelector('.topic-chars');
    const suggestions = document.querySelectorAll('.suggestion');
    const submitButton = document.querySelector('button[type="submit"]');
    const languageSelect = document.querySelector(".languageSelect");
    const toneSelect = document.querySelector(".toneSelect");

    if (!topicInput || !charCountDisplay || !suggestions.length || !submitButton) {
        console.error('One or more elements not found in the DOM');
        return;
    }

    // Function to update character count
    const updateCharCount = () => {
        const currentLength = topicInput.value.length;
        const maxLength = 100;
        charCountDisplay.textContent = `${currentLength}/${maxLength}`;

        // Limit to 100 characters (in case initial value exceeds limit)
        if (currentLength > maxLength) {
            topicInput.value = topicInput.value.substring(0, maxLength);
            charCountDisplay.textContent = `${maxLength}/${maxLength}`;
        }
    };

    // 1. Update character count on page load
    updateCharCount();

    // Update character count on input
    topicInput.addEventListener('input', function () {
        updateCharCount();
    });

    // 2. Suggestion click handler
    suggestions.forEach(suggestion => {
        suggestion.addEventListener('click', function () {
            topicInput.value = this.textContent;
            updateCharCount();
        });
    });

    // Define accents for each language
    const accents = {
        "اللغة العربية": [
            { value: "العربية الفصحة", text: "العربية الفصحة" },
            { value: "اللهجة السورية", text: "اللهجة السورية" },
            { value: "اللهجة المصرية", text: "اللهجة المصرية" },
            { value: "اللهجة اليمنية", text: "اللهجة اليمنية" },
            { value: "اللهجة السعودية", text: "اللهجة السعودية" },
            { value: "اللهجة الخليجية", text: "اللهجة الخليجية" },
            { value: "اللهجة اللبنانية", text: "اللهجة اللبنانية" },
            { value: "اللهجة الأردنية", text: "اللهجة الأردنية" }
        ],
        "اللغة الإنجليزية": [
            { value: "اللهجة الأميركية", text: "اللهجة الأميركية" },
            { value: "اللهجة البريطانية", text: "اللهجة البريطانية" }
        ]
    };

    // Function to update the accents based on selected language
    function updateTones() {
        const selectedLanguage = languageSelect.value;
        const options = accents[selectedLanguage] || [];

        // Clear current options
        toneSelect.innerHTML = "";

        options.forEach((option) => {
            const newOption = document.createElement("option");
            newOption.value = option.value;
            newOption.textContent = option.text;

            // Set default selection: Syrian for Arabic, American for English
            if ((selectedLanguage === "اللغة العربية" && option.value === "اللهجة المصرية") ||
                (selectedLanguage === "اللغة الإنجليزية" && option.value === "اللهجة الأميركية")) {
                newOption.selected = true;
            }

            toneSelect.appendChild(newOption);
        });
    }

    // Listen for language selection changes
    languageSelect.addEventListener("change", updateTones);

    // Set the initial language to Arabic and tone to "اللهجة السورية" on page load
    languageSelect.value = "اللغة العربية"; // Set default language to Arabic
    updateTones(); // Update the accents based on the selected language

    // 4. Submit button animation
    submitButton.addEventListener('click', function (e) {
        const animationDiv = document.createElement('div');
        animationDiv.className = 'submit-animation';
        animationDiv.textContent = 'جاري تجهيز المقابلة...';

        animationDiv.style.cssText = ` 
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            padding: 20px 40px;
            background: var(--color-gradient-1);
            color: var(--color-bg);
            border-radius: 10px;
            font-size: 24px;
            animation: pulse 1.5s infinite;
            z-index: 1000;
            box-shadow: 0 0 20px rgba(0,0,0,0.3);
        `;

        document.body.appendChild(animationDiv);
    });

    // 5. toggle jobs suggestions
    const toggleJobsBtn = document.getElementById("toggleJobs");
    const hiddenSuggestions = document.querySelectorAll(".suggestions .hidden");
    let isExpanded = false;

    toggleJobsBtn.addEventListener("click", function (e) {
        e.preventDefault(); // Prevent page jump

        if (!isExpanded) {
            hiddenSuggestions.forEach(item => item.classList.remove("hidden"));
            toggleJobsBtn.textContent = "عرض أقل"; // Change text to "Show Less"
        } else {
            hiddenSuggestions.forEach(item => item.classList.add("hidden"));
            toggleJobsBtn.textContent = "عرض المزيد"; // Change text to "Show More"
        }

        isExpanded = !isExpanded;
    });
});

// Add keyframes
const styleSheet = document.createElement('style');
styleSheet.textContent = `
    @keyframes pulse {
        0% { transform: translate(-50%, -50%) scale(1); }
        50% { transform: translate(-50%, -50%) scale(1.05); }
        100% { transform: translate(-50%, -50%) scale(1); }
    }
`;
document.head.appendChild(styleSheet);
