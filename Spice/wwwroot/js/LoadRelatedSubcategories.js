function UpdateSubCategoryList() {
    var SelectedCategory = document.getElementById("ddlCategoryList").value;

    $list = $('#SubCategoryList');

    $.ajax({
        url: '@Url.Action("GetSubCategory", "SubCategory")/' + SelectedCategory,
        type: 'GET',
        data: 'text',
        success: function (data) {
            results = data;
            $list.html('');
            $list.append(' <ul class="list-group"> ');
            for (i in results) {
                $list.append('<li class="list-group-item"> ' + results[i].text + ' </li>');
            }
            $list.append('</ul>');
        }
    });
}

$(document).ready(function () {
    UpdateSubCategoryList();
});

$("#ddlCategoryList").on("change", function () {
    UpdateSubCategoryList();
});