document.addEventListener("DOMContentLoaded", () => {
    const btn = document.getElementById("highlightBtn");
    btn.addEventListener("click", async () => {
        const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
        await chrome.scripting.executeScript({
            target: { tabId: tab.id },
            files: ["content-script.js"]
        });
    });
    const saveBtn = document.getElementById("saveBtn");
    saveBtn.addEventListener("click", async () => {
        const [tab] = await chrome.tabs.query({ active: true, currentWindow: true });

        await chrome.scripting.executeScript({
            target: { tabId: tab.id },
            func: () => {
                if (typeof save === "function") {
                    save();
                } else {
                    console.error("Função save() não encontrada no contexto da página.");
                }
            }
        });
    });

});
