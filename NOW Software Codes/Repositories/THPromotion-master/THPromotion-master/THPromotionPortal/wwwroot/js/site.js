function createCountryCodeElement() {
    var tag = document.createElement("div class='form-control row'");
    tag.innerHTML = '<div><input id="ab" class="form-control row" /></div>' +
        '<div><input id="addBtn" value="Submit & Add Next" type="button" onclick="AddAnotherCountryCodeElement()" class="btn btn-primary" /></div>';
    var element = document.getElementById("Divv");
    element.appendChild(tag);
    document.getElementById("selecttt").disabled = true;
}

function AddAnotherCountryCodeElement() {
    document.getElementById("selecttt").disabled = false;
}