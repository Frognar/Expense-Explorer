// Enter in a text field with its suggestion list open picks the highlighted suggestion (the autocomplete
// does that on key up) instead of submitting the form. Without this the browser submits on key down,
// so the form was saved with the half-typed text before the suggestion could be picked.
document.addEventListener("keydown", event => {
    const field = event.target;
    if ((event.key === "Enter" || event.key === "NumpadEnter")
        && field instanceof HTMLInputElement
        && field.getAttribute("role") === "combobox"
        && field.getAttribute("aria-expanded") === "true") {
        event.preventDefault();
    }
}, true);
