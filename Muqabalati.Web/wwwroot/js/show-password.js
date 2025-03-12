var showPasswordTrgs = document.querySelectorAll(".showPasswordTrg");
console.log(showPasswordTrgs);

showPasswordTrgs.forEach(trg => {
    trg.addEventListener("click", e => {
        const inputPassword = trg.closest(".input-box").querySelector("input[type='password'], input[type='text']");

        console.log(inputPassword);
        if (inputPassword) {
            if (inputPassword.type === "password") {
                inputPassword.type = "text";
                trg.innerHTML = `
                  <svg class="showPasswordTrg"  width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M12 4.5C7 4.5 3.00002 8.5 1.50002 12C3.00002 15.5 7 19.5 12 19.5C17 19.5 21 15.5 22.5 12C21 8.5 17 4.5 12 4.5Z" stroke="#A27B5C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                    <path d="M12 15.5C14.4853 15.5 16.5 13.4853 16.5 11C16.5 8.51472 14.4853 6.5 12 6.5C9.51472 6.5 7.5 8.51472 7.5 11C7.5 13.4853 9.51472 15.5 12 15.5Z" stroke="#A27B5C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                    <path d="M12 8.5C10.62 8.5 9.5 9.62 9.5 11C9.5 12.38 10.62 13.5 12 13.5C13.38 13.5 14.5 12.38 14.5 11C14.5 9.62 13.38 8.5 12 8.5Z" stroke="#A27B5C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
                    `;
            } else {
                inputPassword.type = "password";
                trg.innerHTML = `
            <svg class="showPasswordTrg"  width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M14.53 9.47001L9.47004 14.53C8.82004 13.88 8.42004 12.99 8.42004 12C8.42004 10.02 10.02 8.42001 12 8.42001C12.99 8.42001 13.88 8.82001 14.53 9.47001Z" stroke="#A27B5C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                        <path d="M17.82 5.77001C16.07 4.45001 14.07 3.73001 12 3.73001C8.46997 3.73001 5.17997 5.81001 2.88997 9.41001C1.98997 10.82 1.98997 13.19 2.88997 14.6C3.67997 15.84 4.59997 16.91 5.59997 17.77" stroke="#A27B5C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                        <path opacity="0.4" d="M8.42004 19.53C9.56004 20.01 10.77 20.27 12 20.27C15.53 20.27 18.82 18.19 21.11 14.59C22.01 13.18 22.01 10.81 21.11 9.39999C20.78 8.87999 20.42 8.38999 20.05 7.92999" stroke="#A27B5C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                        <path opacity="0.4" d="M15.5099 12.7C15.2499 14.11 14.0999 15.26 12.6899 15.52" stroke="#A27B5C" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                        <path d="M9.47 14.53L2 22" stroke="#2C3930" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                        <path d="M22 2L14.53 9.47" stroke="#2C3930" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
        `;
            }
        }
    })
});
