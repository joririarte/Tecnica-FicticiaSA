document.querySelector('form').addEventListener('submit', async function (e) {
    e.preventDefault(); // Evita el envío del formulario por defecto

    const correo = document.getElementById('email').value;
    const clave = document.getElementById('password').value;

    if (!correo || !clave) {
      Swal.fire({
        icon: "info",
        title: "Oops...",
        text: "Por favor completa los campos",
      });
      return;
    }

    try {
      const response = await fetch('http://localhost:5075/api/Acceso/Login', { // Cambia por tu URL
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ correo, clave }),
      });

      const data = await response.json();

      if (data.isSuccess) {
        // Guardar el token JWT en localStorage
        localStorage.setItem('token', data.token);

        // Redirigir al dashboard
        window.location.href = '../../Sistema/dashboard.html';
      } else {
        Swal.fire({
          icon: "info",
          title: "Error al iniciar sesión.",
          text: data.message,
        });
      }
    } catch (error) {
      console.error('Error al iniciar sesión:', error);
      Swal.fire({
        icon: "error",
        title: "Oops...",
        text: "Hubo un error al intentar iniciar sesión. Inténtalo de nuevo.",
      });
    }
  });