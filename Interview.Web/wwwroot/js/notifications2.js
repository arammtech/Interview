// Pass your Razor TempData values into JavaScript
var successMessage = '@TempData["success"]'; // Make sure TempData is populated in the view
var errorMessage = '@TempData["error"]';
var warningMessage = '@TempData["warning"]';
var infoMessage = '@TempData["info"]';
var welcomeMessage = '@TempData["welcome"]';
var loginMessage = '@TempData["login"]';

// For success message
if (successMessage) {
    createToast("success", '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2" class="icon success-icon" viewBox="0 0 24 24"><path d="M9 11l3 3L22 4" stroke-linecap="round" stroke-linejoin="round"></path></svg>', "Success", successMessage);
}

// For error message
if (errorMessage) {
    createToast("error", '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2" class="icon success-icon" viewBox="0 0 24 24"><path d="M9 11l3 3L22 4" stroke-linecap="round" stroke-linejoin="round"></path></svg>', "Error", errorMessage);
}

// For warning message
if (warningMessage) {
    createToast("warning", '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2" class="icon success-icon" viewBox="0 0 24 24"><path d="M9 11l3 3L22 4" stroke-linecap="round" stroke-linejoin="round"></path></svg>', "Warning", warningMessage);
}

// For info message
if (infoMessage) {
    createToast("info", '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2" class="icon success-icon" viewBox="0 0 24 24"><path d="M9 11l3 3L22 4" stroke-linecap="round" stroke-linejoin="round"></path></svg>', "Info", infoMessage);
}

// Optional: Check for additional messages like welcome or login
if (welcomeMessage) {
    createToast("info", '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="none" stroke="currentColor" viewBox="0 0 24 24" stroke-width="2" class="icon success-icon" viewBox="0 0 24 24"><path d="M9 11l3 3L22 4" stroke-linecap="round" stroke-linejoin="round"></path></svg>', "Welcome", welcomeMessage);
}

if (loginMessage) {
    createToast("info", infoSvg, "Login", loginMessage);
}
}
function createToast(type, svg, title, text) {
    let newToast = document.createElement("div");
    newToast.innerHTML = `
        <div class="toast ${type}">
            <div class="icon">${svg}</div>
            <div class="content">
                <div class="title">${title}</div>
                <span>${text}</span>
            </div>
            <i class="fa-solid fa-xmark" onclick="(this.parentElement).remove()"></i>
        </div>`;
    const notifications = document.querySelector('.notifications');
    if (notifications) {
        notifications.appendChild(newToast);
    }
    newToast.timeOut = setTimeout(() => newToast.remove(), 5000);
}