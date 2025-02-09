async function waitForElement(selector, timeout = 10000) {
    return new Promise((resolve, reject) => {
        const startTime = Date.now();

        function check() {
            const element = document.querySelector(selector);
            if (element) {
                resolve(element);
            } else if (Date.now() - startTime < timeout) {
                requestAnimationFrame(check);
            } else {
                reject(new Error(`Timeout waiting for: ${selector}`));
            }
        }

        check();
    });
}
async function waitForElements(selector, timeout = 10000) {
    return new Promise((resolve, reject) => {
        const startTime = Date.now();

        function checkElements() {
            const elements = document.querySelectorAll(selector);
            if (elements.length > 0) {
                resolve(Array.from(elements)); // Ensure it always returns an array
            } else if (Date.now() - startTime < timeout) {
                requestAnimationFrame(checkElements);
            } else {
                reject(new Error("Timeout waiting for elements: " + selector));
            }
        }

        checkElements();
    });
}

async function waitForNetworkResponse(urlKeyword, timeout = 10000) {
    return new Promise((resolve, reject) => {
        const startTime = Date.now();
        let cleanupDone = false;
        let originalXHROpen;

        function cleanup() {
            if (cleanupDone) return;  
            cleanupDone = true;
            clearInterval(interval);
            XMLHttpRequest.prototype.open = originalXHROpen; // Restore original
        }

        function checkResponse(event) {
            try {
                const requestUrl = event.target.responseURL;
                if (!requestUrl.includes(urlKeyword)) return;

                const response = event.target.responseText;
                const extractedData = extractReportData(response);
                console.log(extractedData)
                cleanup();
                resolve(extractedData);
            } catch (error) {
                cleanup();
                reject(error);
            }
        }

        function monitorRequests() {
            originalXHROpen = XMLHttpRequest.prototype.open;
            XMLHttpRequest.prototype.open = function () {
                this.addEventListener("load", checkResponse);
                originalXHROpen.apply(this, arguments);
            };
        }

        monitorRequests();

        const interval = setInterval(() => {
            if (Date.now() - startTime > timeout) {
                cleanup();
                reject(new Error("Timeout waiting for network response"));
            }
        }, 500);
    });
}



function interceptRequest() {
    // Store original fetch API
    const originalFetch = window.fetch;

    // Override fetch to intercept and modify
    window.fetch = async function (input, init) {
        const url = typeof input === 'string' ? input : input.url;

        if (url.includes('requestreport')) {
            try {
                // Clone request to read body
                const clone = input.clone();
                const bodyText = await clone.text();
                const modifiedBody = modifyPayload(bodyText);

                // Create new request with modified body
                const newRequest = new Request(input, {
                    body: modifiedBody
                });

                return originalFetch(newRequest);
            } catch (e) {
                console.error('Error modifying fetch request:', e);
            }
        }
        return originalFetch.apply(this, arguments);
    };

    // Store original XHR methods
    const originalOpen = XMLHttpRequest.prototype.open;
    const originalSend = XMLHttpRequest.prototype.send;

    // Override XHR open to capture URL
    XMLHttpRequest.prototype.open = function (method, url) {
        this._url = url;
        originalOpen.apply(this, arguments);
    };

    // Override XHR send to intercept and modify
    XMLHttpRequest.prototype.send = function (body) {
        if (this._url.includes('requestreport')) {
            try {
                body = modifyPayload(body);
            } catch (e) {
                console.error('Error modifying XHR payload:', e);
            }
        }
        originalSend.call(this, body);
    };

    // Date modification logic
    function modifyPayload(body) {
        const payload = JSON.parse(body);

        if (payload.params) {
            payload.params = payload.params.map(param => {
                if (param.name === 'startDate') {
                    // Fixed start date
                    return { ...param, value: '01/15/2025' };
                }
                if (param.name === 'endDate') {
                    // Dynamic end date (tomorrow)
                    const tomorrow = new Date();
                    tomorrow.setDate(tomorrow.getDate() + 1);
                    const formattedDate =
                        String(tomorrow.getMonth() + 1).padStart(2, '0') + '/' +
                        String(tomorrow.getDate()).padStart(2, '0') + '/' +
                        tomorrow.getFullYear();

                    return { ...param, value: formattedDate };
                }
                return param;
            });
        }

        return JSON.stringify(payload);
    }
}


function extractReportData(response) {
    try {
        // Parse the JSON response
        const jsonResponse = JSON.parse(response);
        if (!jsonResponse.data) {
            console.error("No data field in response.");
            return null;
        }
        // Convert the HTML string into a DOM document
        const parser = new DOMParser();
        const doc = parser.parseFromString(jsonResponse.data, "text/html");

        const extractedData = {};

        // Loop over every section (each .performance-chart represents a section)
        const charts = doc.querySelectorAll(".performance-chart");
        charts.forEach(chart => {
            // Get the section title from the header (h5 element)
            const sectionTitle = chart.querySelector("h5") ? chart.querySelector("h5").textContent.trim() : "Unknown Section";

            // If the section is "Trades", process it as a table of trade objects
            if (sectionTitle.toLowerCase() === "trades") {
                const tradesArray = [];
                const table = chart.querySelector("table");
                if (table) {
                    // Extract headers from the table head
                    const headers = [];
                    table.querySelectorAll("thead tr th").forEach(th => {
                        headers.push(th.textContent.trim());
                    });
                    // Loop through each row in the table body and create an object
                    table.querySelectorAll("tbody tr").forEach(row => {
                        const tradeObj = {};
                        const cells = row.querySelectorAll("td");
                        headers.forEach((header, index) => {
                            tradeObj[header] = cells[index] ? cells[index].textContent.trim() : "";
                        });
                        tradesArray.push(tradeObj);
                    });
                }
                extractedData[sectionTitle] = tradesArray;
            } else {
                // For other sections, extract key/value pairs from rows with .name and .value
                const sectionData = {};
                chart.querySelectorAll("tr").forEach(row => {
                    const keyElem = row.querySelector("td.name");
                    const valueElem = row.querySelector("td.value");
                    if (keyElem && valueElem) {
                        const key = keyElem.textContent.trim();
                        const value = valueElem.textContent.trim();
                        sectionData[key] = value;
                    }
                });
                extractedData[sectionTitle] = sectionData;
            }
        });
        return extractedData;
    } catch (error) {
        console.error("Error extracting report data:", error);
        return null;
    }
}



async function clickTargetButton() {
    try {
        await waitForElement('.btn.btn-icon'); 
        await new Promise(r => setTimeout(r, 2000)); 

        const buttons = Array.from(document.querySelectorAll('.btn.btn-icon'));
        const targetButton = buttons.find(button => button.querySelector('.icon-columns'));

        if (targetButton) {
            targetButton.click();
            console.log('Clicked on the correct button to expand account details.');
        } else {
            console.error('Could not find the specific column button.');
        }
    } catch (error) {https://trader.tradovate.com/#
        console.error("Error in clicking button:", error);
    }
}

async function getName() {
    try {
        const accountElement = await waitForElement('.account .name div');
        const accountName = accountElement.textContent.trim();
        console.log("Step 1: Account Name:", accountName);

        return accountName;
    } catch (error) {
        console.error(error);
        return null;
    }
}


async function getEquity() {
    try {
        const elements = await waitForElements('.col-xs-12');
        const targetElement = elements.find(el => {
            const labelElement = el.querySelector("label.label.text-muted.align-label");
            return labelElement && labelElement.textContent.trim() === "Net Liquidity";
        });

        if (!targetElement) {
            console.warn("Net Liquidity label not found.");
            return "Not found";
        }

        // Get the value from the same div
        const valueElement = targetElement.querySelector(".val");
        if (!valueElement || !valueElement.childNodes[0]) {
            console.warn("Value element not found inside .val");
            return "Not found";
        }

        const equity = valueElement.childNodes[0].textContent.trim();


        console.log("Net Liquidity:", equity);
        return equity;
    } catch (error) {
        console.error("Error fetching equity:", error);
        return 'Not found';
    }
}


async function getInfo() {
    try {
        const goButton = await waitForElement('.btn.btn-outline.btn-lg');

        if (goButton) {
            interceptRequest();
            const reportdata = waitForNetworkResponse("requestreport");
            await new Promise(r => setTimeout(r, 2000)); // 1-second delay
            const event = new MouseEvent("click", { bubbles: true, cancelable: false });
            goButton.dispatchEvent(event);
        } else {
            console.warn("Button was not found.");
        }
    } catch (error) {
        console.error("Error clicking button:", error);
    }
}


(async () => {
    if (window.dataSent) return;

    try {
        const accountName = await getName();
        if (!accountName) return;

        clickTargetButton();
        const equity = await getEquity();

        getInfo(); // If this is async, it should have await

        // Properly await the network response
        const performanceData = await waitForNetworkResponse('requestreport', 15000);

        // Combine all data AFTER all promises resolve
        const finalData = {
            accountName,
            equity,
            performanceData // Now contains actual data
        };

        window.chrome.webview.postMessage(finalData);
        window.dataSent = true;

    } catch (err) {
        console.error('Script error:', err);
        window.chrome.webview.postMessage({
            error: err.message
        });
    }
})();
