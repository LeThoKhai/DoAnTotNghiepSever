
    $(document).ready(function () {
        var pageSize = 2; // Số lượng dòng mỗi trang
    var pageCount = Math.ceil($("tbody tr").length / pageSize); // Tính tổng số trang

    for (var i = 0; i < pageCount; i++) {
        $('#pagination').append('<a class="page-link" href="#">' + (i + 1) + '</a> ');
        }

    $("tbody tr").hide(); // Ẩn tất cả các dòng
    $("tbody tr").slice(0, pageSize).show(); // Hiển thị số dòng của trang đầu tiên

    $('#pagination a:first').addClass('active');

    $('#pagination a').click(function (e) {
        e.preventDefault();
    $('#pagination a').removeClass('active');
    $(this).addClass('active');
    var page = $(this).text() - 1;
    var start = page * pageSize;
    var end = start + pageSize;
    $("tbody tr").hide().slice(start, end).show();
        });
    });