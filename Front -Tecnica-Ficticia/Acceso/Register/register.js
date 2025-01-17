document.querySelector('form').addEventListener('submit', async function (e) {
    e.preventDefault(); // Prevenir el comportamiento por defecto del formulario

    const name = document.getElementById('name').value;
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    if (!name || !email || !password) {
        Swal.fire({
            icon: "info",
            title: "Oops...",
            text: "Por favor completa los campos",
        });
        return;
    }

    const requestBody = {
        "nombre": name,
        "correo": email,
        "clave": password
    };

    try {
        const response = await fetch('http://localhost:5075/api/Acceso/Registrarse', { // Cambia la URL a la de tu API
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(requestBody),
        });

        const data = await response.json();

        if (data.isSuccess) {
            Swal.fire({
                title: "Éxito",
                text: "Se ha registrado el usuario!",
                icon: "success"
              });
            window.location.href = '../Login/login.html'; // Redirigir al login después del registro
        } else {
            Swal.fire({
                icon: "info",
                title: "Error al registrar el usuario",
                text: data.message,
              });
        }
    } catch (error) {
        console.error('Error al registrar el usuario:', error);
        Swal.fire({
            icon: "error",
            title: "Oops...",
            text: "Hubo un error al intentar registrar el usuario. Inténtalo de nuevo.",
          });
    }
});