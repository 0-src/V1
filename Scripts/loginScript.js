let attempts = 0;
let interval = setInterval(() => {
    let usernameField = document.querySelector("#name-input");
    let passwordField = document.querySelector("#password-input");
    let loginButton = document.querySelector(".MuiButton-root.MuiButton-contained.fat-button.full-width-button");

    if (usernameField && passwordField && loginButton) {
        function setInputValue(field, value) {
            let nativeInputValueSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, 'value').set;
            nativeInputValueSetter.call(field, value);
            field.dispatchEvent(new Event('input', { bubbles: true }));
        }

        setInputValue(usernameField, "{{Username}}");
        setInputValue(passwordField, "{{Password}}");


        if (usernameField.value === "{{Username}}" && passwordField.value === "{{Password}}") {
            setTimeout(() => {
                loginButton.click();

                // Wait for the Access Simulation button after login
                let accessAttempts = 0;
                let accessInterval = setInterval(() => {
                    let accessButton = document.querySelector(".MuiButton-root.jss9.MuiButton-contained.fat-button.full-width-button.mt-2.mb-2.tm");

                    if (accessButton) {
                        accessButton.click();
                        clearInterval(accessInterval);
                    }

                    if (++accessAttempts > 10) {
                        clearInterval(accessInterval);
                    }
                }, 500); // Check every 500ms for Access Simulation button

            }, 2000); // Delay clicking login to ensure values persist

            clearInterval(interval);
        }
    }

    if (++attempts > 10) {
        clearInterval(interval);
    }
}, 500); // Check every 500ms
