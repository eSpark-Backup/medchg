let lastActivityTime = Date.now();
let inactivityLimit = 2592000000; // 6 hours in milliseconds
let inactivitycheck = 2505600000; // 5 hours in milliseconds
function resetTimeout() {
    lastActivityTime = Date.now();
    console.log("Time Reset Of Activity at:- " + new Date().getHours() + " " + new Date().getMinutes() + " " + new Date().getSeconds());
}

function checkInactivity() {
    let element = document.getElementById('signOutButton');
    if (Date.now() - lastActivityTime > inactivityLimit) {
        // Notify Blazor to log out the user
        if (element)
        {
            console.log("Logout Of InActivity :-" + new Date().getHours() + " " + new Date().getMinutes() + " " + new Date().getSeconds());
                element.click();
            }
    }
}
function IncreaseSession()
{
    let inactivityLimit = 2592000000;
    let inactivitycheck = 2505600000; // 5 hours in milliseconds
}
// Attach resetTimeout to user activity events
window.onload = resetTimeout;
window.onkeypress = resetTimeout;
window.onscroll = resetTimeout;
window.onclick = resetTimeout;

// Check for inactivity every 5 hours
setInterval(checkInactivity, inactivitycheck); // 5 hours in milliseconds

