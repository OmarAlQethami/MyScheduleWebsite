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

function onConfirmClick() {

    const hiddenField = document.getElementById('hdnSelectedSubjects');

    if (!hiddenField) {
        console.error("Hidden field not found!");
        return false;
    }

    hiddenField.value = selectedSubjects.join(',');
}

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

        var slotId = element.getAttribute("data-slot-id");
        if (slotId) {
            var level = element.getAttribute("data-slot-level");
            var electiveNumber = slotId.split('_')[2];

            var slot = document.createElement("div");
            slot.id = slotId;
            slot.className = "subject elective-slot";
            slot.setAttribute("data-level", level);
            slot.setAttribute("onclick", "showElectivePopup(" + level + ", '" + slotId + "')");
            slot.innerHTML = "<span>Elective (" + electiveNumber + ")</span>";

            element.parentNode.replaceChild(slot, element);
        }
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
    console.log("Current selection:", selectedSubjects);
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
        `Elective College Hours Selected: ${selectedCollege} of ${12}`;
    document.getElementById(lblElectiveUniversityHoursSelectedId).innerText =
        `Elective University Hours Selected: ${selectedUniversity} of ${4}`;
}

function showElectivePopup(level, slotId) {
    var electiveCodes = electiveOptions[level] || [];
    var electiveOptionsList = [];

    electiveCodes.forEach(function (code) {
        var electiveName = subjectNameMap[code] || code;
        electiveOptionsList.push({ code: code, name: electiveName });
    });

    var popup = document.createElement("div");
    popup.className = "elective-popup";

    electiveOptionsList.forEach(function (option) {
        var optionDiv = document.createElement("div");
        optionDiv.className = "subject elective-option";
        optionDiv.innerText = option.name;
        optionDiv.onclick = function (e) {
            e.stopPropagation();
            var slot = document.getElementById(slotId);
            var level = slot.getAttribute("data-level");

            var subjectElement = document.createElement("div");
            subjectElement.id = option.code;
            subjectElement.className = "subject history";
            subjectElement.setAttribute("onclick", "SubjectClicked(this)");
            subjectElement.innerHTML = "<span>" + option.name + "</span>";
            subjectElement.setAttribute("data-slot-id", slotId);
            subjectElement.setAttribute("data-slot-level", level);

            slot.parentNode.replaceChild(subjectElement, slot);

            SubjectClicked(subjectElement);

            if (popup && popup.parentNode) {
                popup.parentNode.removeChild(popup);
            }
        };
        popup.appendChild(optionDiv);
    });

    var slotElem = document.getElementById(slotId);
    var rect = slotElem.getBoundingClientRect();
    popup.style.position = "absolute";
    popup.style.top = (rect.bottom + window.scrollY) + "px";
    popup.style.left = (rect.left + window.scrollX) + "px";
    popup.style.backgroundColor = "#EEEEEE";
    popup.style.border = "1px solid #ccc";
    popup.style.padding = "10px";
    popup.style.zIndex = 1000;
    popup.style.boxShadow = "0 4px 8px rgba(0,0,0,0.1)";
    document.body.appendChild(popup);

    document.addEventListener("click", function handler(event) {
        if (!popup.contains(event.target) && event.target.id !== slotId) {
            if (popup && popup.parentNode) {
                popup.parentNode.removeChild(popup);
            }
            document.removeEventListener("click", handler);
        }
    });

    if (slotElem) {
        var childSpans = slotElem.getElementsByTagName("span");
        for (var i = 0; i < childSpans.length; i++) {
            childSpans[i].style.pointerEvents = "none";
        }
    }
}

function restoreSelection(hiddenFieldId) {
    const hiddenField = document.getElementById(hiddenFieldId);
    if (hiddenField && hiddenField.value) {
        hiddenField.value.split(',').forEach(code => {
            const el = document.getElementById(code);
            if (el) el.classList.add('history-selected');
        });
    }
}

function displayAlert(message) {
    var lblOutput = document.getElementById(lblOutputClientId);
    lblOutput.innerText = message;
    lblOutput.style.color = 'red';
}