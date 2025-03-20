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
                <svg class="showPasswordTrg"
  width="24"
  height="25"
  viewBox="0 0 24 25"
  fill="none"
  xmlns="http://www.w3.org/2000/svg"
>
  <!-- Outer eye shape -->
  <path
    d="M2.88984 12.4673C5.17984 8.86733 8.46984 6.78733 11.9998 6.78733C15.5299 6.78733 18.8199 8.86733 21.1099 12.4673C18.8199 16.0673 15.5299 18.1473 11.9998 18.1473C8.46984 18.1473 5.17984 16.0673 2.88984 12.4673Z"
    stroke="#ADB2B1"
    stroke-width="1.5"
    stroke-linecap="round"
    stroke-linejoin="round"
  />
  <!-- Iris/pupil -->
  <path
    d="M11.9998 14.4373C13.3232 14.4373 14.3998 13.3606 14.3998 12.0373C14.3998 10.714 13.3232 9.63733 11.9998 9.63733C10.6765 9.63733 9.59984 10.714 9.59984 12.0373C9.59984 13.3606 10.6765 14.4373 11.9998 14.4373Z"
    stroke="#ADB2B1"
    stroke-width="1.5"
    stroke-linecap="round"
    stroke-linejoin="round"
  />
</svg>

                    `;
            } else {
                inputPassword.type = "password";
                trg.innerHTML = `
             <svg class="showPasswordTrg" width="24" height="25" viewBox="0 0 24 25" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M14.5299 10.0073L9.46992 15.0673C8.81992 14.4173 8.41992 13.5273 8.41992 12.5373C8.41992 10.5573 10.0199 8.95734 11.9999 8.95734C12.9899 8.95734 13.8799 9.35734 14.5299 10.0073Z" stroke="#ADB2B1" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                        <path d="M17.8198 6.30733C16.0698 4.98733 14.0698 4.26733 11.9998 4.26733C8.46984 4.26733 5.17984 6.34733 2.88984 9.94733C1.98984 11.3573 1.98984 13.7273 2.88984 15.1373C3.67984 16.3773 4.59984 17.4473 5.59984 18.3073" stroke="#ADB2B1" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                        <path opacity="0.4" d="M8.41992 20.0673C9.55992 20.5473 10.7699 20.8073 11.9999 20.8073C15.5299 20.8073 18.8199 18.7273 21.1099 15.1273C22.0099 13.7173 22.0099 11.3473 21.1099 9.93735C20.7799 9.41735 20.4199 8.92735 20.0499 8.46735" stroke="#ADB2B1" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                        <path opacity="0.4" d="M15.5104 13.2374C15.2504 14.6474 14.1004 15.7974 12.6904 16.0574" stroke="#ADB2B1" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                        <path d="M9.47 15.0674L2 22.5374" stroke="#ADB2B1" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                        <path d="M22.0003 2.53735L14.5303 10.0074" stroke="#ADB2B1" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                    </svg>
        `;
            }
        }
    })
});
