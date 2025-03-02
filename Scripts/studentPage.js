var selectedSubjects = [];
var totalCompulsoryHours = 0;
var totalElectiveUniversityHours = 0;
var totalElectiveCollegeHours = 0;
var lblHoursTakenId, lblElectiveUniversityHoursTakenId, lblElectiveCollegeHoursTakenId;

document.addEventListener('DOMContentLoaded', () => {
    updateHours();
    updateProgressBar();
});

function SubjectClicked(element) {
    const subjectCode = element.id;
    const lblOutput = document.getElementById(lblOutputClientId);
    lblOutput.innerText = "";

    if (element.classList.contains('taken')) {
        displayAlert("This subject has already been taken");
        return;
    }

    if (selectedSubjects.includes(subjectCode)) {
        selectedSubjects = selectedSubjects.filter(code => code !== subjectCode);
        element.classList.remove('selected');
    }
    selectedSubjects.push(subjectCode);
    element.classList.add('selected');

    updateHours();
    updateProgressBar();
}

function updateHours() {
    let compulsory = 0, university = 0, college = 0;

    selectedSubjects.forEach(code => {
        const hours = subjectCreditHoursMap[code];
        switch (subjectTypeMap[code]) {
            case 1:
            case 2:
                compulsory += hours;
                break;
            case 3:
                college += hours;
                break;
            case 4:
                university += hours;
                break;
        }
    });

    document.getElementById(lblHoursTakenId).textContent =
        `Compulsory Hours Selected: ${compulsory} of ${totalCompulsoryHours}`;
    document.getElementById(lblElectiveCollegeHoursTakenId).textContent =
        `Elective College Hours Selected: ${college} of ${totalElectiveCollegeHours}`;
    document.getElementById(lblElectiveUniversityHoursTakenId).textContent =
        `Elective University Hours Selected: ${university} of ${totalElectiveUniversityHours}`;
}

function showElectivePopup(level, slotId) {
    const electiveOptions = window.electiveOptions[level] || [];
    const popup = document.createElement('div');
    popup.className = 'elective-popup';

    electiveOptions.forEach(code => {
        const option = document.createElement('div');
        option.className = 'elective-option';
        option.textContent = subjectNameMap[code];
        option.onclick = () => {
            const slot = document.getElementById(slotId);
            const newSubject = document.createElement('div');
            newSubject.className = 'subject available';
            newSubject.id = code;
            newSubject.innerHTML = `<span>${subjectNameMap[code]}</span>`;
            newSubject.onclick = () => SubjectClicked(newSubject);
            slot.parentNode.replaceChild(newSubject, slot);
            popup.remove();
            SubjectClicked(newSubject);
        };
        popup.appendChild(option);
    });

    document.body.appendChild(popup);
    const rect = document.getElementById(slotId).getBoundingClientRect();
    popup.style.position = 'absolute';
    popup.style.top = `${rect.bottom + window.scrollY}px`;
    popup.style.left = `${rect.left + window.scrollX}px`;

    setTimeout(() => {
        document.addEventListener('click', function closePopup(e) {
            if (!popup.contains(e.target)) {
                popup.remove();
                document.removeEventListener('click', closePopup);
            }
        });
    }, 0);
}

function updateProgressBar() {
    const totalSelected = selectedSubjects.reduce((sum, code) =>
        sum + subjectCreditHoursMap[code], 0);
    const progressBar = document.getElementById(progressBarId);
    const hoursLabel = document.getElementById(hoursLabelId);

    if (progressBar) {
        progressBar.style.width = `${Math.min((totalSelected / 18) * 100, 100)}%`;
    }
    if (hoursLabel) {
        hoursLabel.textContent = `Hours selected: ${totalSelected}`;
    }
}

function displayAlert(message) {
    var lblOutput = document.getElementById(lblOutputClientId);
    lblOutput.innerText = message;
    lblOutput.style.color = 'red';
}