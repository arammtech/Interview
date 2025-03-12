//overlay.style.display="none";

document.addEventListener('DOMContentLoaded', function () {

    //// After 3.5 seconds, start the fade-out animation
    //setTimeout(() => {
    //    const overlay = document.getElementById('dashboardOverlay');
    //    overlay.style.animation = "fadeOutOverlay 0.5s forwards";

    //    // Remove the overlay from the DOM after the fade-out completes
    //    setTimeout(() => {
    //        if (overlay && overlay.parentNode) {
    //            overlay.parentNode.removeChild(overlay);
    //        }
    //    }, 500);
    //}, 3500);


    // Function to hide the spinner and show content after the page loads
    document.getElementById('loading-spinner').style.display = 'none';
    document.getElementById('content').style.display = 'block';


    // modeSwitch when click swich mode from body light, dark and replace icon (sum , moon)
    const theme = localStorage.getItem("theme");
    const modeSwitch = document.getElementById("modeSwitch");
    const body = document.body;

    if (theme) {
        if (theme == 'dark') {
            body.classList.add("dark");
            modeSwitch.innerHTML = `
          <svg id="icon" width="24" height="25" viewBox="0 0 24 25" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M2.03003 12.92C2.39003 18.07 6.76003 22.26 11.99 22.49C15.68 22.65 18.98 20.93 20.96 18.22C21.78 17.11 21.34 16.37 19.97 16.62C19.3 16.74 18.61 16.79 17.89 16.76C13 16.56 9.00003 12.47 8.98003 7.63996C8.97003 6.33996 9.24003 5.10996 9.73003 3.98996C10.27 2.74996 9.62003 2.15996 8.37003 2.68996C4.41003 4.35996 1.70003 8.34996 2.03003 12.92Z" stroke="#A27B5C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
          </svg>
        `;
        } else {
            body.classList.add("light");
            modeSwitch.innerHTML = `
          <svg id="icon" width="24" height="25" viewBox="0 0 24 25" fill="none" xmlns="http://www.w3.org/2000/svg">
            <path d="M12 4.5V2M12 22.5V20M4.93 4.93L3.51 3.51M20.49 20.49L19.07 19.07M2 12H4.5M22 12H19.5M4.93 19.07L3.51 20.49M20.49 3.51L19.07 4.93M16 12A4 4 0 1 1 12 8A4 4 0 0 1 16 12Z" stroke="#A27B5C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
        `;
        }
    }

    modeSwitch.onclick = () => {

        if (body.classList.contains("light")) {
            body.classList.replace("light", "dark");
            localStorage.setItem("theme", "dark");
            modeSwitch.innerHTML = `
            <svg id="icon" width="24" height="25" viewBox="0 0 24 25" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M2.03003 12.92C2.39003 18.07 6.76003 22.26 11.99 22.49C15.68 22.65 18.98 20.93 20.96 18.22C21.78 17.11 21.34 16.37 19.97 16.62C19.3 16.74 18.61 16.79 17.89 16.76C13 16.56 9.00003 12.47 8.98003 7.63996C8.97003 6.33996 9.24003 5.10996 9.73003 3.98996C10.27 2.74996 9.62003 2.15996 8.37003 2.68996C4.41003 4.35996 1.70003 8.34996 2.03003 12.92Z" stroke="#A27B5C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
            </svg>
        `;

        } else {
            body.classList.replace("dark", "light");
            localStorage.setItem("theme", "light");

            modeSwitch.innerHTML = `
            <svg id="icon" width="24" height="25" viewBox="0 0 24 25" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M12 4.5V2M12 22.5V20M4.93 4.93L3.51 3.51M20.49 20.49L19.07 19.07M2 12H4.5M22 12H19.5M4.93 19.07L3.51 20.49M20.49 3.51L19.07 4.93M16 12A4 4 0 1 1 12 8A4 4 0 0 1 16 12Z" stroke="#A27B5C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
        `;
        }
    }

    // Control the Welcome-Overelay Diaplay
    const overlay = document.getElementById("dashboardOverlay");
    const leaveBtn = document.querySelector(".leaveDashboard");
  
      // Check if dashboardVisited flag exists and equals "1"
      const visited = sessionStorage.getItem("dashboardVisited");

      if (!visited || visited === "0") {
        // Show overlay (it is in HTML so it will display)
        setTimeout(() => {
          overlay.style.animation = "fadeOutOverlay 0.5s forwards";
          // After fade-out, remove overlay from DOM
          setTimeout(() => {
            if (overlay && overlay.parentNode) {
              overlay.parentNode.removeChild(overlay);
            }
          }, 500);
        }, 3500);
        // Set session flag so the animation won't appear again during this session
        sessionStorage.setItem("dashboardVisited", "1");
      } else {
        // If the flag is already set, immediately remove/hide the overlay
        if (overlay && overlay.parentNode) {
          overlay.parentNode.removeChild(overlay);
        }
      }

      // When the "Leave Dashboard" button is clicked, reset the flag
      leaveBtn.addEventListener("click", function() {
        sessionStorage.setItem("dashboardVisited", "0");
      });


    // FullScreen
    const icon = document.getElementById('fullscreen-icon');

    icon.onclick = () => {
        if (!document.fullscreenElement) {
            document.documentElement.requestFullscreen();
        } else {
            if (document.exitFullscreen) {
                document.exitFullscreen();
            }
        }
    };

    // Toggle Sidebar
    const toggleButton = document.getElementById('toggle-button');
    const sidebar = document.querySelector('.sidebar');

    toggleButton.onclick = () => {
        sidebar.classList.toggle('hide');
    };


});

