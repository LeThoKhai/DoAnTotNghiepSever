const apiBaseUrl = 'https://localhost:7076/api/APICategories';

async function getCategories() {
    const response = await fetch(apiBaseUrl, {
        method: 'GET',
        headers: {
            'Authorization': 'Bearer YOUR_JWT_TOKEN' // Replace with your actual token
        }
    });
    const categories = await response.json();
    const categoryList = document.getElementById('categoryList');
    categoryList.innerHTML = '';

    categories.forEach(category => {
        const listItem = document.createElement('li');
        listItem.textContent = category.categoryName; // Assuming your category has a 'name' property
        categoryList.appendChild(listItem);

        // Add Edit and Delete buttons
        const editButton = document.createElement('button');
        editButton.textContent = 'Edit';
        editButton.onclick = () => editCategory(category.categoryId);
        listItem.appendChild(editButton);

        const deleteButton = document.createElement('button');
        deleteButton.textContent = 'Delete';
        deleteButton.onclick = () => deleteCategory(category.categoryId);
        listItem.appendChild(deleteButton);
    });
}
// Hàm lấy category theo categoryId
async function getCategoryById(categoryId) {
    try {
        const response = await fetch(`${apiBaseUrl}/${categoryId}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer YOUR_JWT_TOKEN' // Replace with your actual token
            }
        });
        if (response.ok) {
            const category = await response.json();
            console.log('Category found:', category);
            // Bạn có thể xử lý category này hoặc hiển thị nó lên giao diện tùy theo yêu cầu
        } else {
            console.error(`Error fetching category ${categoryId}:`, response.status);
        }
    } catch (error) {
        console.error('Error:', error);
    }
}

async function addCategory() {
    const name = document.getElementById('categoryName').value;

    const response = await fetch(apiBaseUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer YOUR_JWT_TOKEN' // Replace with your actual token
        },
        body: JSON.stringify({ name }) // Adjust according to your Category model
    });

    if (response.ok) {
        getCategories();
        document.getElementById('categoryName').value = ''; // Clear input
    } else {
        alert('Error adding category');
    }
}

async function editCategory(id) {
    const newName = prompt("Enter new name for the category:");
    if (newName) {
        const response = await fetch(`${apiBaseUrl}/${id}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer YOUR_JWT_TOKEN' // Replace with your actual token
            },
            body: JSON.stringify({ categoryId: id, name: newName }) // Adjust according to your Category model
        });

        if (response.ok) {
            getCategories();
        } else {
            alert('Error updating category');
        }
    }
}

async function deleteCategory(id) {
    const response = await fetch(`${apiBaseUrl}/${id}`, {
        method: 'DELETE',
        headers: {
            'Authorization': 'Bearer YOUR_JWT_TOKEN' // Replace with your actual token
        }
    });

    if (response.ok) {
        getCategories();
    } else {
        alert('Error deleting category');
    }
}

window.onload = getCategories;
