function previewImage(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            const img = document.getElementById('img-preview');
            const icon = document.getElementById('placeholder-icon');
            img.src = e.target.result;
            img.style.display = 'block';
            icon.style.display = 'none';
            document.getElementById('file-name').innerText = input.files[0].name;
        }
        reader.readAsDataURL(input.files[0]);
    }
}

function toggleText(checkbox) {
    const label = document.querySelector(`label[for="${checkbox.id}"]`);
    if (label) {
        if (checkbox.checked) {
            label.innerText = "Desmarcar";
        } else {
            label.innerText = "Selecionar";
        }
    }
}