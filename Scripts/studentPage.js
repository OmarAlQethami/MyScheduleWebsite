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

let takenElectiveSlots = new Set();

function SubjectClicked(element) {
    const subjectCode = element.id;
    const lblOutput = document.getElementById(lblOutputClientId);
    lblOutput.innerText = "";

    if (selectedSubjects.includes(subjectCode)) {
        selectedSubjects = selectedSubjects.filter(code => code !== subjectCode);
        element.classList.remove('selected');

        const slotId = element.dataset.slotId;
        if (slotId) {
            takenElectiveSlots.delete(slotId);

            if (element.dataset.originalSlotHTML) {
                const tempDiv = document.createElement('div');
                tempDiv.innerHTML = element.dataset.originalSlotHTML;
                const restoredSlot = tempDiv.firstElementChild;

                // Note to self, this condition down here is wrong, it shouldn't be hardcoded, but we'll keep it for now until it is implemented in the DB.
                if (element.dataset.level === "10") {
                    restoredSlot.dataset.slotType = "level10";
                    restoredSlot.dataset.maxSlots = "2";
                }

                restoredSlot.onclick = function () {
                    showElectivePopup(
                        parseInt(this.dataset.level),
                        this
                    );
                };
                element.parentNode.replaceChild(restoredSlot, element);
            }
        } else {
            element.className = element.dataset.originalClasses || '';
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
            // Note to self, these conditions down here is wrong, it shouldn't be hardcoded, but we'll keep it for now until it is implemented in the DB.
            const isLevel10 = element.dataset.level === "10";
            const slotId = isLevel10 ?
                `level10-slot-${Date.now()}` :
                `${element.dataset.level}-${Date.now()}`;

            if (!isLevel10 && takenElectiveSlots.has(slotId)) {
                displayAlert("This elective slot is already filled.");
                return;
            }

            if (isLevel10) {
                const level10Selections = Array.from(takenElectiveSlots).filter(id => id.startsWith('level10'));
                if (level10Selections.length >= 2) {
                    displayAlert("Maximum 2 electives allowed in level 10");
                    return;
                }
            }

            element.dataset.slotId = slotId;
            takenElectiveSlots.add(slotId);
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
        collegeHoursElement.textContent = `Elective College Hours Selected: ${college} of ${12}`;
    }

    const universityHoursElement = document.getElementById(lblElectiveUniversityHoursTakenId);
    if (universityHoursElement) {
        universityHoursElement.textContent = `Elective University Hours Selected: ${university} of ${4}`;
    }
}

function showElectivePopup(level, slotElement) {
    const currentLevel = parseInt(slotElement.dataset.currentLevel) || 0;

    // Note to self, this variable down here is wrong, it shouldn't be hardcoded, but we'll keep it for now until it is implemented in the DB.
    const isLevel10 = level === 10;
    const maxSelections = isLevel10 ? 2 : 1;

    const electiveOptions = window.electiveOptions[level] || [];
    const takenSubjects = Array.from(document.querySelectorAll('.subject.taken'))
        .map(el => el.id);

    const popup = document.createElement('div');
    popup.className = 'elective-popup';

    electiveOptions.forEach(code => {
        const option = document.createElement('div');
        option.id = code;
        const prerequisites = subjectPrerequisites[code] || [];

        const hasAllPrerequisites = prerequisites.every(p => takenSubjects.includes(p));
        const existingSelections = Array.from(popup.children).filter(opt =>
            selectedSubjects.includes(opt.id) || takenSubjects.includes(opt.id)
        ).length;

        const isOffered = window.subjectOfferedMap[code];
        const isAvailable = isOffered &&
            subjectLevelMap[code] <= currentLevel &&
            !takenSubjects.includes(code) &&
            !selectedSubjects.includes(code) &&
            hasAllPrerequisites &&
            existingSelections < maxSelections;

        const isNotOffered = !isOffered &&
            subjectLevelMap[code] <= currentLevel &&
            !takenSubjects.includes(code) &&
            !selectedSubjects.includes(code) &&
            hasAllPrerequisites &&
            existingSelections < maxSelections;

        option.className = `subject elective-option ${isOffered ? (isAvailable ? 'available' : 'unavailable') : 'unoffered'}`;
        option.textContent = subjectNameMap[code];

        if (isAvailable || isNotOffered) {
            option.onclick = () => {
                if (!slotElement || !slotElement.parentNode) {
                    return;
                }

                const subjectHours = subjectCreditHoursMap[code] || 0;
                const currentHours = selectedSubjects.reduce((sum, c) =>
                    sum + (subjectCreditHoursMap[c] || 0), 0);

                if (currentHours + subjectHours > 20) {
                    displayAlert("You have reached the maximum limit of 20 hours.");
                    return;
                }

                const newSubject = document.createElement('div');
                if (isAvailable) {
                    newSubject.className = 'subject available selected';
                } else if (isNotOffered) {
                    newSubject.className = 'subject unoffered selected';
                }
                
                newSubject.id = code;
                newSubject.innerHTML = `<span>${subjectNameMap[code]}</span>`;

                newSubject.dataset.originalSlotHTML = slotElement.outerHTML;
                newSubject.dataset.slotId = slotElement.id;
                newSubject.dataset.level = slotElement.dataset.level;
                newSubject.dataset.currentLevel = currentLevel;

                newSubject.onclick = () => SubjectClicked(newSubject);

                slotElement.parentNode.replaceChild(newSubject, slotElement);

                popup.remove();
                selectedSubjects.push(code);
                updateHiddenField();
                updateHours();
                updateProgressBar();
            };
        }
        popup.appendChild(option);
    });

    if (slotElement) {
        const rect = slotElement.getBoundingClientRect();
        popup.style.position = 'absolute';
        popup.style.top = `${rect.bottom + window.scrollY}px`;
        popup.style.left = `${rect.left + window.scrollX}px`;
        popup.style.backgroundColor = '#EEEEEE';
    }

    document.body.appendChild(popup);

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

let hideTimeout;

document.addEventListener('DOMContentLoaded', () => {
    const isFirstView = document.getElementById(lblHoursTakenId) && document.getElementById('subjectTooltip');
    if (isFirstView) {
        const tooltip = document.getElementById('subjectTooltip');

        document.addEventListener('mouseover', function (e) {
            const subject = e.target.closest('.subject');
            if (!subject) return;

            clearTimeout(hideTimeout);
            tooltip.style.display = 'block';
            void tooltip.offsetHeight;

            const code = subject.id;
            if (!subjectNameMap[code]) return;

            document.getElementById('ttSubjectName').textContent = subjectNameMap[code];
            document.getElementById('ttLevel').textContent = subjectLevelMap[code];
            document.getElementById('ttCredits').textContent = subjectCreditHoursMap[code];
            document.getElementById('ttType').textContent = getTypeName(subjectTypeMap[code]);

            const prereqList = document.getElementById('ttPrerequisites');
            prereqList.innerHTML = '';
            const prereqs = subjectPrerequisites[code] || [];
            if (prereqs.length === 0) {
                const li = document.createElement('li');
                li.className = 'tt-prerequisite-item tt-no-prereq';
                li.textContent = 'No Prerequisites';
                prereqList.appendChild(li);
            } else {
                prereqs.forEach(prereq => {
                    const li = document.createElement('li');
                    li.className = 'tt-prerequisite-item';
                    li.textContent = subjectNameMap[prereq] || prereq;
                    prereqList.appendChild(li);
                });
            }

            const statusElement = document.getElementById('ttStatus');
            const status = subject.classList.contains('taken') ? 'Taken' :
                subject.classList.contains('available') ? 'Available' :
                    subject.classList.contains('unoffered') ? 'Unoffered' :
                        subject.classList.contains('unavailable') ? 'Unavailable' : '';
            statusElement.textContent = status;
            statusElement.className = `tt-status status-${status.toLowerCase()}`;

            const rect = subject.getBoundingClientRect();
            const verticalCenter = rect.top + window.scrollY + (rect.height / 2) - (tooltip.offsetHeight / 2);
            tooltip.style.top = `${verticalCenter}px`;
            tooltip.style.left = `${rect.right + window.scrollX + 5}px`;

            setTimeout(() => tooltip.classList.add('visible'), 10);
        });

        document.addEventListener('mouseout', function (e) {
            const subject = e.target.closest('.subject');
            if (!subject) return;

            hideTimeout = setTimeout(() => {
                tooltip.classList.remove('visible');
                setTimeout(() => {
                    tooltip.style.display = 'none';
                }, 200);
            }, 300);
        });

        tooltip.addEventListener('mouseenter', () => clearTimeout(hideTimeout));
        tooltip.addEventListener('mouseleave', () => {
            tooltip.classList.remove('visible');
            setTimeout(() => {
                tooltip.style.display = 'none';
            }, 200);
        });
    }
});

function getTypeName(typeId) {
    switch (typeId) {
        case 1: return 'Compulsory (Major)';
        case 2: return 'Compulsory (University)';
        case 3: return 'College Elective';
        case 4: return 'University Elective';
        default: return 'Unknown';
    }
}

function updateHiddenField() {
    var hiddenField = document.getElementById('hdnSelectedSubjects');
    hiddenField.value = selectedSubjects.join(',');
}

const selectedSections = {};

function SectionClicked(element) {
    displayAlert2('');

    const subjectCode = element.getAttribute('data-subject-code');
    const sectionNumber = element.id;

    const currentSelected = selectedSections[subjectCode];
    const section = window.allSections.find(s => s.SectionNumber.toString() === sectionNumber);

    if (currentSelected === sectionNumber) {
        delete selectedSections[subjectCode];
        element.classList.remove('selected');
        const sectionToRemove = window.allSections.find(s =>
            s.SubjectCode === subjectCode &&
            s.SectionNumber.toString() === sectionNumber
        );
        if (sectionToRemove) {
            removeSubjectFromSchedule(sectionToRemove);
        }
    } else {
        const conflicts = [];

        Object.entries(selectedSections).forEach(([existingSubjCode, existingSectionNum]) => {
            const existingSection = window.allSections.find(s =>
                s.SubjectCode === existingSubjCode &&
                s.SectionNumber.toString() === existingSectionNum
            );

            if (existingSection && sectionsConflict(section, existingSection)) {
                conflicts.push(existingSection.SubjectEnglishName);
            }
        });

        if (conflicts.length > 0) {
            displayAlert2(`Time conflict with: ${conflicts.join(', ')}`);
            return;
        }

        if (currentSelected) {
            const oldSection = window.allSections.find(s =>
                s.SubjectCode === subjectCode &&
                s.SectionNumber.toString() === currentSelected
            );
            if (oldSection) {
                removeSubjectFromSchedule(oldSection);
            }
            const prevSection = document.getElementById(currentSelected);
            if (prevSection) prevSection.classList.remove('selected');
        }

        selectedSections[subjectCode] = sectionNumber;
        element.classList.add('selected');
        addSubjectToSchedule(section);
    }

    const subjectElement = document.getElementById(subjectCode);
    if (subjectElement) {
        subjectElement.classList.toggle('done', !!selectedSections[subjectCode]);
    }

    document.getElementById('hdnSelectedSections').value =
        JSON.stringify(selectedSections);
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
    const subjectName = element.querySelector('span').textContent;

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
            : `<div class="no-sections-message">
                   <div class="section-heading">No sections available for ${subjectName}</div>
                    <div class="section-notice">As it is unoffered this semester</div>
                   <div class="section-notice">You will be placed on a waitlist upon confirming.</div>
               </div>`;

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

function generateScheduleGrid() {
    const grid = document.getElementById('scheduleGrid');
    if (!grid) return;

    grid.innerHTML = '';

    grid.appendChild(createHeaderCell(''));
    for (let hour = 8; hour <= 22; hour++) {
        const timeLabel = `${hour % 12 || 12} ${hour >= 12 ? 'PM' : 'AM'}`;
        const header = createHeaderCell(timeLabel);
        header.innerHTML = `<span>${timeLabel}</span>`;
        grid.appendChild(header);
    }

    const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday'];
    days.forEach((day, dayIndex) => {
        grid.appendChild(createDayLabel(day));
        for (let hour = 8; hour <= 22; hour++) {
            const cell = document.createElement('div');
            cell.className = 'hour-cell';
            cell.dataset.day = dayIndex;
            cell.dataset.hour = hour;
            grid.appendChild(cell);
        }
    });
}

function createHeaderCell(text) {
    const cell = document.createElement('div');
    cell.className = 'grid-header';
    cell.textContent = text;
    return cell;
}

function createDayLabel(text) {
    const cell = document.createElement('div');
    cell.className = 'day-label';
    cell.textContent = text;
    return cell;
}

document.addEventListener('DOMContentLoaded', generateScheduleGrid);

const subjectColors = [
    "#9ec2e6", "#f4b083", "#fed966", "#a7d08c", "#aeaaa9",
    "#cecdae", "#D5A6BD", "#C5A1D9", "#7FC7C0", "#BFAF98"
];

const assignedColors = {};

function getSubjectColor(subjectName) {
    if (!assignedColors[subjectName]) {
        const colorIndex = Object.keys(assignedColors).length % subjectColors.length;
        assignedColors[subjectName] = subjectColors[colorIndex];
    }
    return assignedColors[subjectName];
}

function addSubjectToSchedule(section) {
    const container = document.querySelector('.schedule-container');
    const grid = document.getElementById('scheduleGrid');

    section.Details.forEach(detail => {
        const dayIndex = detail.Day - 1;
        const start = parseTime(detail.StartTime);
        const end = parseTime(detail.EndTime);

        const sampleCell = grid.querySelector(`.hour-cell[data-day="${dayIndex}"][data-hour="8"]`);
        const cellWidth = sampleCell.offsetWidth;
        const cellHeight = sampleCell.offsetHeight;

        const startHour = start.getHours();
        const endHour = end.getHours();
        const duration = endHour - startHour;

        const startColumn = startHour - 8;

        const blockLeft = 100 + (startColumn * cellWidth) + (startColumn * 1);

        const blockTop = (dayIndex + 1) * (cellHeight + 1);

        const block = document.createElement('div');
        block.className = 'scheduled-block';
        block.dataset.sectionId = section.SectionNumber;
        block.style.width = `${(duration * cellWidth) + (duration - 1)}px`;
        block.style.height = `${cellHeight}px`;
        block.style.left = `${blockLeft}px`;
        block.style.top = `${blockTop}px`;

        const subjectColor = getSubjectColor(section.SubjectEnglishName);
        block.style.backgroundColor = subjectColor;

        const content = document.createElement('div');
        content.className = 'scheduled-block-content';
        content.textContent = section.SubjectEnglishName;
        block.appendChild(content);

        container.appendChild(block);
    });
}

function removeSubjectFromSchedule(section) {
    const container = document.querySelector('.schedule-container');
    section.Details.forEach(detail => {
        container.querySelectorAll(`.scheduled-block[data-section-id="${section.SectionNumber}"]`)
            .forEach(block => block.remove());
    });
}

function parseTime(timeString) {
    const [hours, minutes] = timeString.split(':').map(Number);
    return new Date(2000, 0, 1, hours, minutes);
}

function sectionsConflict(a, b) {
    return a.Details.some(aDetail =>
        b.Details.some(bDetail =>
            aDetail.Day === bDetail.Day &&
            timesOverlap(aDetail.StartTime, aDetail.EndTime, bDetail.StartTime, bDetail.EndTime)
        )
    );
}

function timesOverlap(start1, end1, start2, end2) {
    const toMinutes = time => {
        const [hours, minutes] = time.split(':');
        return parseInt(hours) * 60 + parseInt(minutes);
    };

    const s1 = toMinutes(start1);
    const e1 = toMinutes(end1);
    const s2 = toMinutes(start2);
    const e2 = toMinutes(end2);

    return s1 < e2 && e1 > s2;
}

const modal = document.getElementById('recommendationModal');
if (modal) {
    modal.style.display = 'flex';
}

const closeButton = document.querySelector('.close-button');
if (closeButton) {
    closeButton.addEventListener('click', () => {
        modal.style.display = 'none';
    });
}

document.addEventListener('DOMContentLoaded', () => {
    const hdn = document.getElementById('hdnRecommendedSubjects').value;
    const currentLevel = parseInt(document.getElementById('hdnCurrentLevel').value);
    const isFirstView = document.getElementById('subjectTooltip');

    if (isFirstView) {

        try {
            const recommendedSubjects = hdn ? JSON.parse(hdn) : [];
            const level10Electives = [];
            const otherElectives = [];

            // Note to self, this condition down here is wrong, it shouldn't be hardcoded, but we'll keep it for now until it is implemented in the DB.
            recommendedSubjects.forEach(code => {
                const strCode = String(code);
                if ([3, 4].includes(subjectTypeMap[strCode])) {
                    const level = subjectLevelMap[strCode];
                    if (level === 10) {
                        level10Electives.push(strCode);
                    } else {
                        otherElectives.push(strCode);
                    }
                }
            });

            const level10Slots = document.querySelectorAll('.elective-slot[data-level="10"]');
            let slotsFilled = 0;

            level10Electives.forEach((strCode, index) => {
                if (slotsFilled >= 2) return;
                const slot = level10Slots[index];

                // Note to self, this condition down here is wrong, it shouldn't be hardcoded, but we'll keep it for now until it is implemented in the DB.
                if (slot && !slot.classList.contains('taken')) {
                    const isOffered = window.subjectOfferedMap[strCode];
                    const newSubject = document.createElement('div');
                    newSubject.className = `subject ${isOffered ? 'available' : 'unoffered'} selected`;
                    newSubject.id = strCode;
                    newSubject.innerHTML = `<span>${subjectNameMap[strCode]}</span>`;
                    newSubject.onclick = () => SubjectClicked(newSubject);

                    const slotId = `level10-slot-${index}`;
                    newSubject.dataset.slotId = slotId;
                    newSubject.dataset.originalSlotHTML = slot.outerHTML;
                    newSubject.dataset.level = 10;
                    newSubject.dataset.currentLevel = currentLevel;
                    newSubject.dataset.isLevel10 = true;

                    slot.parentNode.replaceChild(newSubject, slot);
                    selectedSubjects.push(strCode);
                    slotsFilled++;
                }
            });

            otherElectives.forEach(strCode => {
                const level = subjectLevelMap[strCode];
                const slots = document.querySelectorAll(`.elective-slot[data-level="${level}"]`);

                slots.forEach(slot => {
                    if (!slot.classList.contains('taken') && !slot.classList.contains('selected')) {
                        const isOffered = window.subjectOfferedMap[strCode];
                        const newSubject = document.createElement('div');
                        newSubject.className = `subject ${isOffered ? 'available' : 'unoffered'} selected`;
                        newSubject.id = strCode;
                        newSubject.innerHTML = `<span>${subjectNameMap[strCode]}</span>`;
                        newSubject.onclick = () => SubjectClicked(newSubject);

                        newSubject.dataset.originalSlotHTML = slot.outerHTML;
                        newSubject.dataset.slotId = slot.id;
                        newSubject.dataset.level = level;
                        newSubject.dataset.currentLevel = currentLevel;
                        newSubject.dataset.isOffered = isOffered;

                        slot.parentNode.replaceChild(newSubject, slot);
                        selectedSubjects.push(strCode);
                        return;
                    }
                });
            });

            recommendedSubjects.forEach(code => {
                const strCode = String(code);
                if (![3, 4].includes(subjectTypeMap[strCode])) {
                    const subjectElement = document.getElementById(strCode);
                    if (subjectElement && !subjectElement.classList.contains('selected')) {
                        const subjectHours = subjectCreditHoursMap[strCode] || 0;
                        const currentHours = selectedSubjects.reduce((sum, c) =>
                            sum + (subjectCreditHoursMap[c] || 0), 0);

                        if (currentHours + subjectHours <= 20) {
                            SubjectClicked(subjectElement);
                        }
                    }
                }
            });

            updateHiddenField();
            updateHours();
            updateProgressBar();

        } catch (err) {
            displayAlert(`Recommendation error: ${err.message}`);
            console.error("Recommendation error details:", err);
        }
    }
});

function replaceSlotWithSubject(slotElement, subjectCode) {
    const originalHTML = slotElement.outerHTML;
    const isOffered = window.subjectOfferedMap[strCode];
    const newSubject = document.createElement('div');
    newSubject.className = `subject ${isOffered ? 'available' : 'unoffered'} selected`;
    newSubject.id = subjectCode;
    newSubject.innerHTML = `<span>${subjectNameMap[subjectCode]}</span>`;
    newSubject.dataset.originalSlot = originalHTML;
    newSubject.onclick = () => SubjectClicked(newSubject);
    slotElement.parentNode.replaceChild(newSubject, slotElement);
}

function displayAlert(message) {
    const lblOutput = document.getElementById(lblOutputClientId);
    if (lblOutput) {
        lblOutput.innerText = message;
        lblOutput.style.color = 'red';
    }
}

function displayAlert2(message) {
    const lblOutput2 = document.getElementById(lblOutput2ClientId);
    if (lblOutput2) {
        lblOutput2.innerText = message;
        lblOutput2.style.color = 'red';
    }
}