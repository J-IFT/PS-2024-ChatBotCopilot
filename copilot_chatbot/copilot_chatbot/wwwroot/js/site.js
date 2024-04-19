// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function handleKeyPress(event) {
    if (event.key === 'Enter') {
        sendMessage();
    }
}

async function sendMessage() {
    var promptInput = document.getElementById("prompt-input").value;
    
    var responseContainer = document.getElementById("response");

    responseContainer.innerHTML += `<div class"response-item" style="background-color:#00B0F0; width: 300px; padding: 20px; color: white; margin: 20px;">
                                        ${promptInput}
                                    </div>`;

    try {
        var response = await fetch("/Chat/SendMessage", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ prompt: promptInput })
        });

        if (response.ok) {
            var data = await response.json();
            responseContainer.innerHTML += `<div class"response-bot-item" style="background-color:#0070C0; width: 300px; padding: 20px; color: white; margin: 20px 0 20px 100px;">
                                                ${data.choices[0].message.content}
                                            </div>`;

            document.getElementById("prompt-input").value = "";
        } else {
            responseContainer.innerText += "Erreur lors de la communication avec le serveur!";
        }
    } catch (error) {
        responseContainer.innerText += "Erreur lors de la communication avec le serveur : " + error.message;
    }
}