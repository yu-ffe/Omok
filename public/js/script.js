async function register() {
    const nickname = document.getElementById("nickname").value;
    const password = document.getElementById("password").value;

    const response = await fetch("/auth/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ nickname, password })
    });

    const data = await response.json();
    document.getElementById("registerResult").innerText = data.message;
}

async function login() {
    const nickname = document.getElementById("loginNickname").value;
    const password = document.getElementById("loginPassword").value;

    const response = await fetch("/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ nickname, password })
    });

    const data = await response.json();
    document.getElementById("loginResult").innerText = data.message;

    if (response.ok) {
        localStorage.setItem("token", data.token);
    }
}
