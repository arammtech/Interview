// select #searchInput and track the enter key when pressed to show an alert (Not Implemented Yet)
// and when click on #searchInput svg the same
// modeSwitch when click swich mode from body light, dark and replace icon (sum , moon)
const theme = localStorage.getItem("theme");
const modeSwitch = document.getElementById("modeSwitch");
const body = document.body;

if (theme) {
    if (theme == 'dark') {
        //body.classList.remove("light"); 
        body.classList.add("dark");
        modeSwitch.innerHTML = `
          <svg id="icon" width="24" height="25" viewBox="0 0 24 25" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M2.03003 12.92C2.39003 18.07 6.76003 22.26 11.99 22.49C15.68 22.65 18.98 20.93 20.96 18.22C21.78 17.11 21.34 16.37 19.97 16.62C19.3 16.74 18.61 16.79 17.89 16.76C13 16.56 9.00003 12.47 8.98003 7.63996C8.97003 6.33996 9.24003 5.10996 9.73003 3.98996C10.27 2.74996 9.62003 2.15996 8.37003 2.68996C4.41003 4.35996 1.70003 8.34996 2.03003 12.92Z" stroke="#E5E5E5" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
          </svg>
        `;
    } else {
        body.classList.add("light");
        modeSwitch.innerHTML = `
          <svg id="icon" width="24" height="25" viewBox="0 0 24 25" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M12 4.5V2M12 22.5V20M4.93 4.93L3.51 3.51M20.49 20.49L19.07 19.07M2 12H4.5M22 12H19.5M4.93 19.07L3.51 20.49M20.49 3.51L19.07 4.93M16 12A4 4 0 1 1 12 8A4 4 0 0 1 16 12Z" stroke="#E5E5E5" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        `;
    }
}

document.addEventListener('DOMContentLoaded', function () {

    // Function to hide the spinner and show content after the page loads
    document.getElementById('loading-spinner').style.display = 'none';
    document.getElementById('content').style.display = 'block';

    

    modeSwitch.onclick = () => {

        if (body.classList.contains("light")) {
            body.classList.replace("light", "dark");
            localStorage.setItem("theme", "dark");
            modeSwitch.innerHTML = `
            <svg id="icon" width="24" height="25" viewBox="0 0 24 25" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M2.03003 12.92C2.39003 18.07 6.76003 22.26 11.99 22.49C15.68 22.65 18.98 20.93 20.96 18.22C21.78 17.11 21.34 16.37 19.97 16.62C19.3 16.74 18.61 16.79 17.89 16.76C13 16.56 9.00003 12.47 8.98003 7.63996C8.97003 6.33996 9.24003 5.10996 9.73003 3.98996C10.27 2.74996 9.62003 2.15996 8.37003 2.68996C4.41003 4.35996 1.70003 8.34996 2.03003 12.92Z" stroke="#E5E5E5" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
            </svg>
        `;

        } else {
            body.classList.replace("dark", "light");
            localStorage.setItem("theme", "light");

            modeSwitch.innerHTML = `
            <svg id="icon" width="24" height="25" viewBox="0 0 24 25" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M12 4.5V2M12 22.5V20M4.93 4.93L3.51 3.51M20.49 20.49L19.07 19.07M2 12H4.5M22 12H19.5M4.93 19.07L3.51 20.49M20.49 3.51L19.07 4.93M16 12A4 4 0 1 1 12 8A4 4 0 0 1 16 12Z" stroke="#E5E5E5" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
        `;
        }
    }


    // Arrow up button
    const backToTopBtn = document.getElementById("backToTop");
    window.addEventListener("scroll", function () {
        if (window.scrollY > window.innerHeight) {
            backToTopBtn.classList.add("show");
        } else {
            backToTopBtn.classList.remove("show");
        }
    });
    backToTopBtn.addEventListener("click", function () {
        backToTopBtn.classList.add("rocket");

        setTimeout(() => {
            window.scrollTo({ top: 0, behavior: "smooth" });
            setTimeout(() => {
                backToTopBtn.classList.remove("rocket");
            }, 1000);
        }, 400);
    });

    // help button
    const helpButton = document.getElementById("helpButton");
    window.addEventListener("scroll", function () {
        if (window.scrollY > window.innerHeight) {
            helpButton.classList.add("show");
        } else {
            helpButton.classList.remove("show");
        }
    });
    helpButton.addEventListener("click", function (event) {
        event.preventDefault(); // Prevent the default link behavior temporarily
        helpButton.classList.add("rocket");

        setTimeout(() => {
            // Perform the navigation after the animation completes
            window.location.href = helpButton.getAttribute("href");
        }, 1000); // After animation (1 second), navigate to the link
    });







 });



