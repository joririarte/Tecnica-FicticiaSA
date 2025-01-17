const token = localStorage.getItem('token');
const clientForm = document.getElementById('clientForm');
const modalTitle = document.getElementById('addClientModalLabel');
const modalActionButton = document.getElementById('modalActionButton');
const clientIdInput = document.getElementById('clientId');
var modal = new bootstrap.Modal(document.getElementById('addClientModal'));
let atributoIndex = 1;



async function cargarClientes() {
    if (!token) {
      window.location.href = '../Acceso/Login/login.html'; 
      return;
    }

    try {
      const response = await fetch('http://localhost:5075/api/Cliente/Clientes/', {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`, 
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        throw new Error('No se pudo cargar los clientes');
      }

      const data = await response.json();

      if (data.clientes && data.clientes.length > 0) {
        
        const tableBody = document.getElementById('clientesTableBody');

        tableBody.innerHTML = '';

       
        data.clientes.forEach(cliente => {
          const row = document.createElement('tr');
          
          row.innerHTML = `
            <td>${cliente.clienteId}</td>
            <td>${cliente.clienteNombre}</td>
            <td>${cliente.clienteIdentificacion}</td>
            <td>${cliente.clienteEdad}</td>
            <td>${cliente.clienteGenero}</td>
            <td><input class="form-check-input" type="checkbox" value="${cliente.clienteEstado}" id="flexCheckDefault" ${cliente.clienteEstado ? 'checked':''}></td>
            <td>
                <button class="btn btn-info" onclick="mostrarDetalles(${cliente.clienteId})">Detalles</button>
                <button class="btn btn-warning" data-bs-toggle="modal" data-bs-target="#addClientModal" onclick="openEditClientModal(${cliente.clienteId})">Editar</button>
                <button class="btn btn-danger" data-id="${cliente.clienteId}" onclick="deleteClient(${cliente.clienteId})">Eliminar</button>
            </td>
          `;

          
          tableBody.appendChild(row);
        });
      } else {
        console.error('No se encontraron clientes o hubo un error al obtenerlos');
        Swal.fire({
            icon: "info",
            title: "Sin Datos",
            text: "No se encontraron clientes o hubo un error al obtenerlos",
          });
      }
    } catch (error) {
      console.error(error);
      Swal.fire({
        icon: "error",
        title: "Error",
        text: error,
      });
    }
  }


async function mostrarDetalles(clienteId) {
    try {
        const response = await fetch(`http://localhost:5075/api/Cliente/Clientes/${clienteId}`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error('No se pudo obtener los detalles del cliente');
        }

        const data = await response.json();

        if (data.cliente) {
            document.getElementById('clienteId').textContent = data.cliente.clienteId;
            document.getElementById('clienteNombre').textContent = data.cliente.clienteNombre;
            document.getElementById('clienteIdentificacion').textContent = data.cliente.clienteIdentificacion;
            document.getElementById('clienteEdad').textContent = data.cliente.clienteEdad;
            document.getElementById('clienteGenero').textContent = data.cliente.clienteGenero;
            document.getElementById('clienteManeja').textContent = data.cliente.clienteManeja ? 'Sí' : 'No';
            document.getElementById('clienteLentes').textContent = data.cliente.clienteLentes ? 'Sí' : 'No';
            document.getElementById('clienteDiabetico').textContent = data.cliente.clienteDiabetico ? 'Sí' : 'No';
            document.getElementById('clienteOtros').textContent = data.cliente.clienteOtros ? 'Sí' : 'No';

            
            const atributosList = document.getElementById('atributosAdicionalesList');
            atributosList.innerHTML = ''; 

            data.cliente.atributosAdicionales.forEach(atributo => {
                const listItem = document.createElement('li');
                listItem.textContent = atributo.aaAtributo;
                atributosList.appendChild(listItem);
            });

            const modal = new bootstrap.Modal(document.getElementById('clienteDetallesModal'));
            modal.show();
        } else {
            console.error('No se encontraron detalles del cliente');
            Swal.fire({
                icon: "info",
                title: "Sin Datos",
                text: "No se encontraron detalles del cliente",
              });
        }
    } catch (error) {
        console.error('Error al obtener los detalles del cliente:', error);
        Swal.fire({
            icon: "error",
            title: "Error",
            text: "Error al cargar obtener los detalles del cliente:" + error,
          });
    }
}
function toggleAtributosInput(atributosAdicionales = []) {
    const atributosContainer = document.getElementById('atributosContainer');
    const otrosCheck = document.getElementById('clientOtros');
    const atributosAdicionalesContainer = document.getElementById('atributosAdicionalesContainer');
  
    if (otrosCheck.checked) {
      atributosContainer.style.display = 'block';
      atributosAdicionalesContainer.innerHTML = ''; 
      atributoIndex = 0; 
  
      
      if (atributosAdicionales.length > 0) {
        atributosAdicionales.forEach(atributo => {
          addAtributoInput(atributo.aaAtributo);
        });
      } else {
        addAtributoInput(); 
      }
    } else {
      atributosContainer.style.display = 'none';
      atributosAdicionalesContainer.innerHTML = ''; 
      atributoIndex = 1; 
    }
  }


function addAtributoInput(valor = '') {
    const container = document.getElementById('atributosAdicionalesContainer');
    const newInput = document.createElement('div');
    newInput.classList.add('d-flex', 'mb-3');
    newInput.innerHTML = `
      <input type="text" class="form-control" placeholder="Atributo adicional" id="atributoAdicional${atributoIndex}" value="${valor}">
    `;
    container.appendChild(newInput);
    atributoIndex++;
  }
  

document.getElementById("addClientButton").addEventListener("click", () => {
    modalTitle.textContent = 'Agregar Cliente';
    modalActionButton.textContent = 'Agregar Cliente';
    clientForm.reset(); 
    clientIdInput.value = ''; 
    modalActionButton.onclick = handleAddClient;
});


async function openEditClientModal(clientId) {
    modalTitle.textContent = 'Editar Cliente';
    modalActionButton.textContent = 'Actualizar Cliente';

    const response = await fetch(`http://localhost:5075/api/Cliente/Clientes/${clientId}`, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        }
    });


    const data = await response.json();

    if (data && data.cliente) {
        const cliente = data.cliente;

       
        clientIdInput.value = cliente.clienteId;
        document.getElementById('clientName').value = cliente.clienteNombre;
        document.getElementById('clientIdentificacion').value = cliente.clienteIdentificacion;
        document.getElementById('clientEdad').value = cliente.clienteEdad;
        document.getElementById('clientGenero').value = cliente.clienteGenero;


        document.getElementById('clientEstado').checked = cliente.clienteEstado;
        document.getElementById('clientManeja').checked = cliente.clienteManeja;
        document.getElementById('clientLentes').checked = cliente.clienteLentes;
        document.getElementById('clientDiabetico').checked = cliente.clienteDiabetico;
        document.getElementById('clientOtros').checked = cliente.clienteOtros;

        if(document.getElementById('clientOtros').checked){
            toggleAtributosInput(cliente.atributosAdicionales || []);
        }

    
        modalActionButton.onclick = handleEditClient;
    }
    modal.show();
}

async function handleAddClient(event) {
    event.preventDefault(); 
    const clienteNombre = document.getElementById('clientName').value.trim();
    const clienteIdentificacion = document.getElementById('clientIdentificacion').value.trim();
    const clienteEdad = parseInt(document.getElementById('clientEdad').value, 10);

    if (!clienteNombre) {
        Swal.fire({
            title: "Campo Obligatorio",
            text: "EL nombre es obligatorio",
            icon: "info"
        });
        return;
    }

    if (!clienteIdentificacion) {
        Swal.fire({
            title: "Campo Obligatorio",
            text: "La identificación es obligatoria",
            icon: "info"
        });
        return; 
    }

    if (isNaN(clienteEdad)) {
        Swal.fire({
            title: "Campo Obligatorio",
            text: "La edad debe ser un numero valido",
            icon: "info"
        });
    return; 
    }


    if (clienteEdad < 18 || clienteEdad > 99) {
        Swal.fire({
            title: "Edad fuera de Rango",
            text: "La edad debe estar entre 0 y 99",
            icon: "info"
        });
        return; 
    }

    const clienteGenero = document.getElementById('clientGenero').value;
    const clienteEstado = document.getElementById('clientEstado').checked;
    const clienteManeja = document.getElementById('clientManeja').checked;
    const clienteLentes = document.getElementById('clientLentes').checked;
    const clienteDiabetico = document.getElementById('clientDiabetico').checked;
    const clienteOtros = document.getElementById('clientOtros').checked;
  
    
    const atributosAdicionales = [];
    if (clienteOtros) {
      for (let i = 0; i < atributoIndex; i++) {
        const atributo = document.getElementById(`atributoAdicional${i}`);
        if (atributo && atributo.value.trim()) {
          atributosAdicionales.push({ aaAtributo: atributo.value });
        }
      }
    }
  
    const body = {
      clienteNombre,
      clienteIdentificacion,
      clienteEdad,
      clienteGenero,
      clienteEstado,
      clienteManeja,
      clienteLentes,
      clienteDiabetico,
      clienteOtros,
      atributosAdicionales
    };
  
    try {
        const response = await fetch('http://localhost:5075/api/Cliente/Clientes', {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(body)
        });
  
        if (response.ok) {
            const data = await response.json();
            if (data.isSuccess) {
          
            Swal.fire({
                title: "Éxito",
                text: "Cliente agregado exitosamente",
                icon: "success"
            });

            cargarClientes();

            modal.hide();
        } else {
            Swal.fire({
                title: "Ocurrió un problema",
                text: "Hubo un problema al agregar el cliente",
                icon: "info"
            });
        }
      } else {
            throw new Error('Error al agregar cliente');
      }
    } catch (error) {
        console.error('Error al agregar cliente:', error);
        Swal.fire({
            title: "Error",
            text: "Ocurrió un error al intentar agregar el cliente" + error,
            icon: "error"
        });
    }
    modal.hide();
}


async function handleEditClient(event) {
    event.preventDefault();

    const cliente = {
        clienteId: clientIdInput.value,
        clienteNombre: document.getElementById('clientName').value,
        clienteIdentificacion: document.getElementById('clientIdentificacion').value,
        clienteEdad: parseInt(document.getElementById('clientEdad').value),
        clienteGenero: document.getElementById('clientGenero').value,
        clienteEstado: document.getElementById('clientEstado').checked,
        clienteManeja: document.getElementById('clientManeja').checked,
        clienteLentes: document.getElementById('clientLentes').checked,
        clienteDiabetico: document.getElementById('clientDiabetico').checked,
        clienteOtros: document.getElementById('clientOtros').checked,
        atributosAdicionales: [] 
    };

    if (cliente.clienteOtros) {
        const atributosContainer = document.getElementById('atributosAdicionalesContainer');
        const inputs = atributosContainer.querySelectorAll('input[type="text"]');

        inputs.forEach(input => {
            const aaAtributo = input.value.trim();
            if (aaAtributo) {
                cliente.atributosAdicionales.push({ aaAtributo });
            }
        });
    }

    try{
        const response = await fetch(`http://localhost:5075/api/Cliente/Clientes/${cliente.clienteId}`, {
        method: 'PATCH',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(cliente)
        });
    if (response.ok) {
        const data = await response.json();
        if (data.isSuccess) {
            Swal.fire({
                title: "Éxito",
                text: "Cliente editado exitosamente",
                icon: "success"
            });

        cargarClientes();

        modal.hide();
        } else {
            Swal.fire({
                title: "Ocurrió un problema",
                text: "Hubo un problema al editar el cliente",
                icon: "info"
            });
        }
    } else {
        throw new Error('Error al agregar cliente');
    }
    } catch (error) {
        console.error('Error al agregar cliente:', error);
        Swal.fire({
            title: "Error",
            text: "Ocurrió un error al intentar editar el cliente" + error,
            icon: "error"
        });
    }

  modal.hide();
}


async function deleteClient(clienteId) {
    const result = await Swal.fire({
        title: "¿Quieres eliminar el cliente?",
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: "Eliminar",
        denyButtonText: `No eliminar`
    });
    if (result.isConfirmed) {
        try {
        
        const response = await fetch(`http://localhost:5075/api/Cliente/Clientes/${clienteId}`, {
            method: 'DELETE',
            headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json'
            }
        });

       
        if (response.ok) {
        const data = await response.json();
        if (data.isSuccess) {
            Swal.fire("Eliminado!", "Se ha eliminado el cliente", "success");
            cargarClientes();
        } else {
            Swal.fire({
                title: "Ocurrió un problema",
                text: data.message,
                icon: "info"
            });
        }
        } else {
        throw new Error('Error al eliminar cliente');
        }
        } catch (error) {
            console.error('Error al eliminar cliente:', error);
            Swal.fire({
                title: "Error",
                text: "Ocurrió un error al intentar eliminar el cliente" + error,
                icon: "error"
            });
        }
    }
    else if(result.isDenied){
        Swal.fire("No se han guardado los cambios", "", "info");
    }
  }

window.onload = cargarClientes;

document.getElementById('logoutBtn').addEventListener('click', function () {
    localStorage.removeItem('token');
    window.location.href = '../index.html'; 
});