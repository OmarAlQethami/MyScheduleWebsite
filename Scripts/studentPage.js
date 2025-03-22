var selectedSubjects = [];
var totalCompulsoryHours = 0;
var totalElectiveUniversityHours = 0;
var totalElectiveCollegeHours = 0;
var lblHoursTakenId, lblElectiveUniversityHoursTakenId, lblElectiveCollegeHoursTakenId;

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.subject').forEach(subject => {
        const originalClasses = Array.from(subject.classList)
            .filter(c => c !== 'selected')
            .join(' ');
        subject.dataset.originalClasses = originalClasses;
    });
    updateHours();
    updateProgressBar();
});

function SubjectClicked(element) {
    const subjectCode = element.id;
    const lblOutput = document.getElementById(lblOutputClientId);
    lblOutput.innerText = "";

    if (selectedSubjects.includes(subjectCode)) {
        selectedSubjects = selectedSubjects.filter(code => code !== subjectCode);
        element.classList.remove('selected');

        const slotId = element.dataset.slotId;
        if (slotId) {
            const originalSlot = document.createElement('div');
            originalSlot.id = slotId;
            originalSlot.className = element.dataset.slotClasses;
            originalSlot.innerHTML = element.dataset.slotHtml;
            originalSlot.setAttribute('data-level', element.dataset.level);
            originalSlot.setAttribute('data-current-level', element.dataset.currentLevel);

            const electiveNumber = element.dataset.electiveNumber;
            originalSlot.innerHTML = `<span>Elective (${electiveNumber})</span>`;

            originalSlot.setAttribute(
                'onclick',
                `showElectivePopup(${element.dataset.level}, '${slotId}', ${element.dataset.currentLevel})`

            );

            element.parentNode.replaceChild(originalSlot, element);
        } else {
            element.className = element.dataset.originalClasses;
        }
    } else {
        const subjectHours = subjectCreditHoursMap[subjectCode] || 0;
        const currentHours = selectedSubjects.reduce((sum, code) =>
            sum + (subjectCreditHoursMap[code] || 0), 0);

        if (currentHours + subjectHours > 20) {
            displayAlert("You have reached the maximum limit of 20 hours.");
            return;
        }

        if (element.classList.contains('elective-slot')) {
            element.dataset.slotId = element.id;
            element.dataset.slotClasses = element.className;
            element.dataset.electiveNumber = element.id.split('_').pop();
        }

        selectedSubjects.push(subjectCode);
        element.classList.add('selected');
    }
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

function showElectivePopup(level, slotId, currentLevel) {
    const electiveOptions = window.electiveOptions[level] || [];
    const takenSubjects = Array.from(document.querySelectorAll('.subject.taken'))
        .map(el => el.id);

    const popup = document.createElement('div');
    popup.className = 'elective-popup';

    electiveOptions.forEach(code => {
        const option = document.createElement('div');
        const isAvailable = subjectLevelMap[code] <= currentLevel &&
            !takenSubjects.includes(code) &&
            !selectedSubjects.includes(code);

        option.className = `subject elective-option ${isAvailable ? 'available' : 'unavailable'}`;
        option.textContent = subjectNameMap[code];

        if (isAvailable) {
            option.onclick = () => {
                const slot = document.getElementById(slotId);
                const newSubject = document.createElement('div');
                newSubject.className = 'subject available selected';
                newSubject.id = code;
                newSubject.innerHTML = `<span>${subjectNameMap[code]}</span>`;
                newSubject.onclick = () => SubjectClicked(newSubject);
                newSubject.dataset.slotId = slotId;
                newSubject.dataset.slotClasses = slot.className;
                newSubject.dataset.slotHtml = slot.innerHTML;
                newSubject.dataset.level = slot.dataset.level;
                newSubject.dataset.currentLevel = slot.dataset.currentLevel;
                newSubject.dataset.electiveNumber = slotId.split('_').pop();
                slot.parentNode.replaceChild(newSubject, slot);
                popup.remove();
                newSubject.dataset.originalClasses = 'available';
                selectedSubjects.push(code);
                updateHours();
                updateProgressBar();
            };
        }
        popup.appendChild(option);
    });

    document.body.appendChild(popup);
    const rect = document.getElementById(slotId).getBoundingClientRect();
    popup.style.position = 'absolute';
    popup.style.top = `${rect.bottom + window.scrollY}px`;
    popup.style.left = `${rect.left + window.scrollX}px`;
    popup.style.backgroundColor = '#EEEEEE';

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

    const progressWidth = (totalSelected / 20) * 100;
    if (progressBar) {
        progressBar.style.width = `${Math.min(progressWidth, 100)}%`;
        progressBar.style.backgroundColor = totalSelected < 12 ? 'red' : 'green';
    }
    if (hoursLabel) {
        hoursLabel.textContent = `Hours selected: ${totalSelected}`;
    }
}

function SectionClicked(element) {
    if (!element.classList.contains('selected')) {
        element.classList.add('selected');
    }
    else {
        element.classList.remove('selected');
    }
}

function displayAlert(message) {
    var lblOutput = document.getElementById(lblOutputClientId);
    lblOutput.innerText = message;
    lblOutput.style.color = 'red';
}