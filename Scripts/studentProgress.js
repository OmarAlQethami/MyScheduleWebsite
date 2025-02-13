var selectedSubjects = [];
var totalCompulsoryHours = 0;
var totalElectiveUniversityHours = 0;
var totalElectiveCollegeHours = 0;
var lblHoursSelectedId = '';
var lblElectiveUniversityHoursSelectedId = '';
var lblElectiveCollegeHoursSelectedId = '';

document.addEventListener("DOMContentLoaded", function () {
    if (typeof preSelectedSubjects !== 'undefined' && preSelectedSubjects.length > 0) {
        preSelectedSubjects.forEach(subjectCode => {
            var element = document.getElementById(subjectCode);
            if (element && !selectedSubjects.includes(subjectCode)) {
                selectedSubjects.push(subjectCode);
                element.classList.add('history-selected');
            }
        });
        updateHours();
    }
});

function SubjectClicked(element) {
    var subjectCode = element.id;
    var lblOutput = document.getElementById(lblOutputClientId);
    lblOutput.innerText = "";

    var subjectLevel = subjectLevelMap[subjectCode];
    if (subjectLevel > currentStudentLevel) {
        displayAlert("You cannot take level " + subjectLevel + " subjects.");
        return;
    }

    if (selectedSubjects.includes(subjectCode)) {
        selectedSubjects = selectedSubjects.filter(item => item !== subjectCode);
        element.classList.remove('history-selected');
        RemoveDependentSubjects(subjectCode);
    } else {
        var prerequisites = subjectPrerequisites[subjectCode] || [];
        var missingPrerequisites = [];

        prerequisites.forEach(prereq => {
            var requiredSubjects = prereq.split(',').map(s => s.trim());
            requiredSubjects.forEach(prerequisiteCode => {
                if (!selectedSubjects.includes(prerequisiteCode)) {
                    var prerequisiteName = subjectNameMap[prerequisiteCode] || prerequisiteCode;
                    missingPrerequisites.push(prerequisiteName);
                }
            });
        });

        if (missingPrerequisites.length > 0) {
            displayAlert("You must first take the prerequisite: " + missingPrerequisites.join(", ") + ".");
            return;
        }

        selectedSubjects.push(subjectCode);
        element.classList.add('history-selected');
    }

    updateHours();
}

function RemoveDependentSubjects(deselectedSubject) {
    for (var subject in subjectPrerequisites) {
        var prerequisites = subjectPrerequisites[subject] || [];
        var shouldRemove = prerequisites.some(prereq => {
            var requiredSubjects = prereq.split(',').map(s => s.trim());
            return requiredSubjects.includes(deselectedSubject);
        });

        if (shouldRemove && selectedSubjects.includes(subject)) {
            selectedSubjects = selectedSubjects.filter(item => item !== subject);
            var dependentElement = document.getElementById(subject);
            if (dependentElement) {
                dependentElement.classList.remove('history-selected');
            }
            RemoveDependentSubjects(subject);
        }
    }
}

function updateHours() {
    var selectedCompulsory = 0, selectedUniversity = 0, selectedCollege = 0;

    selectedSubjects.forEach(function (subjectCode) {
        var type = subjectTypeMap[subjectCode];
        var hours = subjectCreditHoursMap[subjectCode];

        if (type === 1 || type === 2) selectedCompulsory += hours;
        else if (type === 3) selectedCollege += hours;
        else if (type === 4) selectedUniversity += hours;
    });

    document.getElementById(lblHoursSelectedId).innerText =
        `Compulsory Hours Selected: ${selectedCompulsory} of ${totalCompulsoryHours}`;
    document.getElementById(lblElectiveCollegeHoursSelectedId).innerText =
        `Elective College Hours Selected: ${selectedCollege} of ${totalElectiveCollegeHours}`;
    document.getElementById(lblElectiveUniversityHoursSelectedId).innerText =
        `Elective University Hours Selected: ${selectedUniversity} of ${totalElectiveUniversityHours}`;
}

function displayAlert(message) {
    var lblOutput = document.getElementById(lblOutputClientId);
    lblOutput.innerText = message;
    lblOutput.style.color = 'red';
}