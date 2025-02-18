const apiBaseUrlCourse = 'https://localhost:7076/api/APICourses';

// Biến để lưu courseId được chọn
let selectedCourseId;

async function getCourses() {
    try {
        const response = await fetch(apiBaseUrlCourse, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbkBnbWFpbC5jb20iLCJqdGkiOiJjMDkwNzFlNC1lZDE2LTRkNzUtYjg4OC03NGRkY2M4YTA1ZDYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjMyMWRiOWFkLTA4ZWMtNDczZS05ZTE2LTNhZGFjYzYzYTMzOSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzI2ODI4MjIxLCJpc3MiOiJXZWJTaXRlSG9jVGllbmdOaGF0IiwiYXVkIjoiV2ViU2l0ZUhvY1RpZW5nTmhhdCJ9.PoYtM-STK7DxjDT3SGhHsq3YH0CR37XgNqWXfRUguJs' // Thay thế bằng token thật
            }
        });

        // Kiểm tra nếu phản hồi không thành công
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        // Parse JSON từ phản hồi
        const courses = await response.json();
        // Tìm phần tử danh sách trong HTML
        const courseList = document.getElementById('courseList');
        courseList.innerHTML = '';

        // Lặp qua các khóa học và thêm vào danh sách
        courses.forEach(course => {
            const listItem = document.createElement('li');
            listItem.innerHTML = `
                <strong>${course.courseName}</strong> - Giá: ${course.price} - Trạng thái: ${course.status ? 'Kích hoạt' : 'Ngừng'}<br>
                <img src="https://localhost:7076${course.image}" alt="${course.courseName}" style="width: 100px; height: auto;">
            `; // Hiển thị thông tin cơ bản và hình ảnh

            // Hiển thị chi tiết hơn khi nhấn vào khóa học
            listItem.onclick = () => {
                selectedCourseId = course.courseId; // Lưu CourseId vào biến bên ngoài
                localStorage.setItem('selectedCourseId', selectedCourseId); // Lưu vào localStorage
                console.log('Đã lưu Course ID:', selectedCourseId);
                
                // Hiển thị chi tiết khóa học
                alert(`Chi tiết Khóa Học:\n- ID: ${course.courseId}\n- Tên: ${course.courseName}\n- Giá: ${course.price}\n- Nội dung: ${course.content}\n- Trạng thái: ${course.status ? 'Kích hoạt' : 'Ngừng'}\n- Hình ảnh: ${course.image || 'Không có hình ảnh'}`);
            };

            courseList.appendChild(listItem);
        });
    } catch (error) {
        console.error('Lỗi khi tải danh sách khóa học:', error);
        alert('Có lỗi xảy ra khi tải danh sách khóa học. Vui lòng thử lại sau.');
    }
}

window.onload = function() {
    getCourses(); // Hàm này sẽ tải danh sách khóa học khi trang được tải
    getCategories(); // Đảm bảo bạn đã định nghĩa hàm này
}
