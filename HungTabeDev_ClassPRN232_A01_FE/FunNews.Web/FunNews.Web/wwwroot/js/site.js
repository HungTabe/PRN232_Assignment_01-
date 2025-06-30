$(document).ready(function () {
    // Xử lý xác nhận xóa với SweetAlert2
    $('form.delete-form').on('submit', function (e) {
        e.preventDefault();
        var form = this;
        Swal.fire({
            title: 'Are you sure?',
            text: 'This action cannot be undone!',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                form.submit();
            }
        });
    });

    // Hiển thị thông báo thành công/lỗi
    function showAlert(message, type = 'success') {
        Swal.fire({
            icon: type,
            title: type === 'success' ? 'Success' : 'Error',
            text: message,
            timer: 3000,
            showConfirmButton: false
        });
    }

    // Gắn hàm showAlert vào global để dùng trong các View
    window.showAlert = showAlert;
});