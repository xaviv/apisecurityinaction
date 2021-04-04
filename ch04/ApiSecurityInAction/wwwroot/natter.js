﻿const apiUrl = 'https://localhost:44364/api';

function createSpace(name, owner) {
    let data = { name: name, owner: owner };
    fetch(apiUrl + '/spaces', {
        method: 'POST',
        credentials: 'include',
        body: JSON.stringify(data),
        headers: {
            'Content-Type': 'application/json'
        }
    }).then(response => {
        if (response.ok) {
            return response.json();
        } else {
            throw Error(response.statusText);
        }
    }).then(json => console.log('Created space: ', json.name, json.uri))
        .catch(error => console.error('Error: ', error));
}

window.addEventListener('load', function (e) {
    document.getElementById('createSpace')
        .addEventListener('submit', processFormSubmit);
});

function processFormSubmit(e) {
    e.preventDefault();

    let spaceName = document.getElementById('spaceName').value;
    let owner = document.getElementById('owner').value;

    createSpace(spaceName, owner);

    return false;
}
