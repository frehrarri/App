function showSuccess(success) {
    const popup = document.getElementById('messagePopup');
    if (!popup) return;

    popup.textContent = success ? 'Saved successfully!' : 'Save failed!';
    popup.classList.remove('hidden');

    setTimeout(() => {
        popup.classList.add('hidden');
    }, 3000);
}
