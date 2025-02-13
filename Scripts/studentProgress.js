var selectedSubjects = [];
var totalHours = 0;

function SubjectClicked(element) {
    var subjectCode = element.id;
    var lblOutput = document.getElementById(lblOutputClientId);

    lblOutput.innerText = "";

    if (selectedSubjects.includes(subjectCode)) {
        selectedSubjects = selectedSubjects.filter(item => item !== subjectCode);
        totalHours -= 3;
        element.classList.remove('history-selected');
    } else if (totalHours + 3 <= 20) {
        selectedSubjects.push(subjectCode);
        totalHours += 3;
        element.classList.add('history-selected');
    } else {
        displayAlert("You can't select more than 20 hours.");
        return;
    }
}

function displayAlert(message) {
    var lblOutput = document.getElementById(lblOutputClientId);
    lblOutput.innerText = message;
    lblOutput.style.color = 'red';
}