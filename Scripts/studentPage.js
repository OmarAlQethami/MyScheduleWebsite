var selectedSubjects = [];
var totalCompulsoryHours = 0;
var totalElectiveUniversityHours = 0;
var totalElectiveCollegeHours = 0;
var lblHoursTakenId, lblElectiveUniversityHoursTakenId, lblElectiveCollegeHoursTakenId;

document.addEventListener('DOMContentLoaded', () => {
    if (document.getElementById(lblHoursTakenId)) {
        document.querySelectorAll('.subject').forEach(subject => {
            const originalClasses = Array.from(subject.classList)
                .filter(c => c !== 'selected')
                .join(' ');
            subject.dataset.originalClasses = originalClasses;
        });
        updateHours();
        updateProgressBar();
    }
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
        updateHiddenField();
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
        updateHiddenField();
    }
    updateHours();
    updateProgressBar();
}

function updateHours() {
    const hoursTakenElement = document.getElementById(lblHoursTakenId);
    if (!hoursTakenElement) return;

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

    hoursTakenElement.textContent = `Compulsory Hours Selected: ${compulsory} of ${totalCompulsoryHours}`;

    const collegeHoursElement = document.getElementById(lblElectiveCollegeHoursTakenId);
    if (collegeHoursElement) {
        collegeHoursElement.textContent = `Elective College Hours Selected: ${college} of ${totalElectiveCollegeHours}`;
    }

    const universityHoursElement = document.getElementById(lblElectiveUniversityHoursTakenId);
    if (universityHoursElement) {
        universityHoursElement.textContent = `Elective University Hours Selected: ${university} of ${totalElectiveUniversityHours}`;
    }
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
                updateHiddenField();
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

function updateHiddenField() {
    var hiddenField = document.getElementById('hdnSelectedSubjects');
    hiddenField.value = selectedSubjects.join(',');
}

const selectedSections = {};

function SectionClicked(element) {
    const subjectCode = element.getAttribute('data-subject-code');
    const sectionNumber = element.id;

    const currentSelected = selectedSections[subjectCode];

    if (currentSelected === sectionNumber) {
        delete selectedSections[subjectCode];
        element.classList.remove('selected');
    } else {
        if (currentSelected) {
            const prevSection = document.getElementById(currentSelected);
            if (prevSection) prevSection.classList.remove('selected');
        }

        selectedSections[subjectCode] = sectionNumber;
        element.classList.add('selected');
    }

    const subjectElement = document.getElementById(subjectCode);
    if (subjectElement) {
        subjectElement.classList.toggle('done', !!selectedSections[subjectCode]);
    }
}

function SubjectInSectionsClicked(element) {
    if (!window.allSections || !Array.isArray(window.allSections)) {
        displayAlert2('Sections data not loaded!');
        return;
    }

    const container = document.getElementById('sectionsContainer');
    if (!container) {
        displayAlert2('Could not find sections container!');
        return;
    }

    const wasSelected = element.classList.contains('selected');

    document.querySelectorAll('.subject-in-sections').forEach(subject => {
        subject.classList.remove('selected');
    });

    if (wasSelected) {
        container.innerHTML = '';
    } else {
        element.classList.add('selected');
        const subjectCode = element.id;
        const sections = window.allSections.filter(s => s.SubjectCode === subjectCode);

        container.innerHTML = sections.length > 0
            ? buildSectionsHtml(sections)
            : '<div class="labels">No sections available</div>';
    }

    if (!wasSelected) {
        const hasSelectedSections = container.querySelectorAll('.selected').length > 0;
        element.classList.toggle('done', hasSelectedSections);
    }
}

function buildSectionsHtml(sections) {
    return sections.map(section => {
        const isSelected = selectedSections[section.SubjectCode] === section.SectionNumber.toString();
        return `
        <div class="section ${isSelected ? 'selected' : ''}" id="${section.SectionNumber}" onclick="SectionClicked(this)" data-subject-code="${section.SubjectCode}">
            <div class="section-rows">
                <div class="section-row">
                    <div class="info-box">
                        <span class="info-label">Section</span>
                        <span class="info-value section-number">${section.SectionNumber}</span>
                    </div>
                    <div class="info-box">
                        <span class="info-label">Subject</span>
                        <span class="info-value subject-name">${section.SubjectEnglishName}</span>
                    </div>
                    <div class="info-box">
                        <span class="info-label">Capacity</span>
                        <span class="info-value capacity">${section.RegisteredStudents}/${section.Capacity}</span>
                    </div>
                </div>
                ${section.Details.map(detail => `
                <div class="section-row">
                    <div class="time-info">
                        <span class="info-label">Day</span>
                        <span class="info-value day">${getDayName(detail.Day)}</span>
                    </div>
                    <div class="time-info">
                        <span class="info-label">From</span>
                        <span class="info-value start-time">${formatTime(detail.StartTime)}</span>
                    </div>
                    <div class="time-info">
                        <span class="info-label">To</span>
                        <span class="info-value end-time">${formatTime(detail.EndTime)}</span>
                    </div>
                    <div class="location">
                        <span class="info-label">Location</span>
                        <span class="info-value location">${detail.Location}</span>
                    </div>
                </div>`).join('')}
                <div class="section-row">
                    <div class="info-box" style="width: 100%;">
                        <span class="info-label">Instructor</span>
                        <span class="info-value instructor-name">${section.InstructorArabicName}</span>
                    </div>
                </div>
            </div>
        </div>`
    }).join('');
}

function getDayName(dayNumber) {
    const days = ['Saturday', 'Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday'];
    return days[dayNumber] || 'Unknown';
}

function formatTime(timeSpan) {
    const date = new Date(`2000-01-01T${timeSpan}`);
    return date.toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit', hour12: true });
}

function displayAlert(message) {
    var lblOutput = document.getElementById(lblOutputClientId);
    lblOutput.innerText = message;
    lblOutput.style.color = 'red';
}

function displayAlert2(message) {
    var lblOutput2 = document.getElementById(lblOutput2ClientId);
    lblOutput2.innerText = message;
    lblOutput2.style.color = 'red';
}