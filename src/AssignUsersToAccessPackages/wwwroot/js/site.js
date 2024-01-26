
let selectedUsers = [];
let selectedAccess = [];

const SELECTED_CLASS = "selected";

const toggleSelectUser = (element, id) => {
    console.log("toggleSelectUser");
    console.log(element);
    console.log(id);

    if (element.classList.contains(SELECTED_CLASS)) {
        element.classList.remove(SELECTED_CLASS);
        const index = selectedUsers.indexOf(id);
        selectedUsers = selectedUsers.splice(index, 1);
    }
    else {
        element.classList.add(SELECTED_CLASS);
        selectedUsers.push(id);
    }

    const inputElement = document.getElementById("selectedUsers");
    inputElement.value = selectedUsers.toString();
};

const toggleSelectAccess = (element, id) => {
    console.log("toggleSelectAccess");
    console.log(element);
    console.log(id);

    if (element.classList.contains(SELECTED_CLASS)) {
        element.classList.remove(SELECTED_CLASS);
        const index = selectedAccess.indexOf(id);
        selectedAccess = selectedAccess.splice(index, 1);
    }
    else {
        element.classList.add(SELECTED_CLASS);
        selectedAccess.push(id);
    }

    const inputElement = document.getElementById("selectedAccesses");
    inputElement.value = selectedAccess.toString();
};
