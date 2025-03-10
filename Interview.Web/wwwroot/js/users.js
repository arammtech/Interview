//import '../lib/toastr.js/toastr.min.js';


let dataTable;

var $ = jQuery.noConflict();
var page = 1;  // Current page
var rowsPerPage = 5;
var totalUsers = 0;
var filterProperty = null;
var filter = null;

document.addEventListener("DOMContentLoaded", function () {

    loadDataTable(
        'admin',
        'user', page, rowsPerPage,
        filterProperty, filter
    );

    const filterButtons = document.querySelectorAll(".filterButton");
    filterButtons.forEach(btn => {
        btn.addEventListener("click", function () {

            filterButtons.forEach(btn => btn.classList.remove("btn-active"));
            this.classList.add("btn-active");

            applyFilter(this.textContent.trim());

        });
    });

    const searchInput = document.getElementById('searchTable');
    const noRecordsRow = document.getElementById('noRecordsRow');

    function filterUsers() {
        
        const filter = searchInput.value.toLowerCase();
        let visibleCount = 0;

        // Select the rows dynamically 
        const rows = document.querySelectorAll("#listTable tbody .userRow");

        rows.forEach(function (row) {
            const name = row.cells[1].textContent.toLowerCase();
            const email = row.cells[2].textContent.toLowerCase();

            if (name.includes(filter) || email.includes(filter)) {
                row.style.display = "";
                visibleCount++;
            } else {
                row.style.display = "none";
            }
        });

        // Show "No Records" message if all rows are hidden
        noRecordsRow.style.display = visibleCount === 0 ? "" : "none";

        // Update pagination
        totalUsers = visibleCount;
        updatePagination();
    }

    // Listen for search input
    searchInput.addEventListener("keyup", filterUsers);

    // apply filter on page load if there's an initial value
    if (searchInput.value.trim() !== "") {
        filterUsers();
    }

    const rowsPerPageSelect = document.getElementById("rowsPerPageSelect");

    // Listen for changes in the dropdown
    rowsPerPageSelect.addEventListener("change", function () {
        rowsPerPage = parseInt(this.value); // Convert to number
        page = 1; // Reset to first page
        loadDataTable('admin', 'user', page, rowsPerPage, filterProperty, filter);
    });


    // Dummy function for the Add User button
    document.getElementById('addBtn').addEventListener('click', function () {
        alert('إضافة مستخدم جديد');
    });



});


function loadDataTable(area, controller, page,rowsPerPage,filterProperty, filter) {
    let url = `/api/${area}/${controller}?page=${page}&rowsPerPage=${rowsPerPage}`;
    //let url = `/${area}/${controller}/GetAll`;
    if (filterProperty && filter) {
        url += `&filterProperty=${filterProperty}&filter=${filter}`;
    }


    // Make the AJAX request
    $.ajax({
        type: 'GET',
        url: url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            let items = response.data ? response.data : response;
            totalUsers = response.totalUsers;

            console.log(items);
            // Clear table before adding new rows
            $("#listTable tbody").empty();

            if (items.length === 0) {
                $("#noRecordsRow").show(); // Show "No Records" row
            } else {
                $("#noRecordsRow").hide(); // Hide "No Records" row
            }


            items.forEach(user => {
                let lockUnlockString = Boolean(user.isLocked) ? "فك القفل" : "قفل";
                let buttonColor = Boolean(user.isLocked) ? "btn-error " : "btn-3";

                let rowHtml = `
    <tr class="userRow">
        <td>${user.id}</td>
        <td>${user.firstName + ' ' + user.lastName}</td>
        <td>${user.email}</td>
        <td>${user.phone || "لا يوجد"}</td>
        <td>${Array.isArray(user.role) ? user.role.join('/') : user.role || "N/A"}</td>
        <td class="row-actions">
            <a onclick="lockUnlock(${user.id})" class="action-btn btn btn-sm ${buttonColor} jump ">${lockUnlockString}</a>
            <a href="/admin/user/editRole/${user.id}" class="action-btn btn btn-sm btn-2 jump ">تعديل الدور</a>
            <a href="/admin/user/details/${user.id}" class="action-btn btn btn-sm btn-acc jump ">تفاصيل</a>
        </td>
    </tr>
    `;

                $("#listTable tbody").append(rowHtml);
            });

            // Update pagination
            updatePagination();

        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", status, error, xhr.responseText);
            Swal.fire("Error", `خطأ في استرجاع البيانات: ${xhr.responseText}`, "error");
        }
    });

};


function updatePagination() {
    let totalPages = Math.ceil(totalUsers / rowsPerPage);
    let paginationHTML = `<ul class="pagination">`;

    // Previous Button
    if (page === 1) {
        paginationHTML += `<li class="page-item disabled"><a class="page-link">Previous</a></li>`;
    } else {
        paginationHTML += `<li class="page-item"><a class="page-link" href="#" onclick="changePage(${page - 1})">Previous</a></li>`;
    }

    // Page Numbers
    for (let i = 1; i <= totalPages; i++) {
        if (i === page) {
            paginationHTML += `<li class="page-item active"><a class="page-link" href="#">${i}</a></li>`;
        } else {
            paginationHTML += `<li class="page-item"><a class="page-link" href="#" onclick="changePage(${i})">${i}</a></li>`;
        }
    }

    // Next Button
    if (page === totalPages) {
        paginationHTML += `<li class="page-item disabled"><a class="page-link">Next</a></li>`;
    } else {
        paginationHTML += `<li class="page-item"><a class="page-link" href="#" onclick="changePage(${page + 1})">Next</a></li>`;
    }

    paginationHTML += `</ul>`;
    document.getElementById("pagination").innerHTML = paginationHTML;
}

function changePage(newPage) {
    if (newPage < 1 || newPage > Math.ceil(totalUsers / rowsPerPage)) return;
    page = newPage;
    loadDataTable('admin', 'user', page, rowsPerPage, filterProperty, filter);
}


function applyFilter(filterValue) {
    switch (filterValue) {
        case "الكل":
            filterProperty = null;
            filter = null;
            break;
        case "عملاء":
            filterProperty = "role";
            filter = "Customer";
            break;
        case "ادمن":
            filterProperty = "role";
            filter = "Admin";
            break;
        case "مقفول":
            filterProperty = "isLocked";
            filter = "true";
            break;
        case "غير مقفول":
            filterProperty = "isLocked";
            filter = "false";
            break;
        default:
            filterProperty = null;
            filter = null;
            break;
    }

    page = 1; // Reset to first page when filtering
    loadDataTable('admin', 'user', page, rowsPerPage, filterProperty, filter);
}



function lockUnlock(userId) {
    $.ajax({
        type: 'POST',
        url: `/api/admin/user/LockUnLock?userId=${userId}`, // Note the explicit route segment
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.success) {
                console.log("Success:", data.message);
                toastr.success(data.message);

                    $("#listTable tbody").empty(); // Clear table before reloading
                loadDataTable('admin', 'user', page, rowsPerPage, filterProperty, filter); // Reload data

            } else {
                console.log("Error:", data.message);
            }
        },
        error: function (xhr, status, error) {
            console.log("AJAX Error:", xhr.responseText);
            toastr.error("An error occurred.");
        }
    });
}





