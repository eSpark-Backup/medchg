document.addEventListener("DOMContentLoaded", function () {
    // Get all date input fields
    let dateInputs = document.querySelectorAll('input[type="date"]');

    // Attach the validateYearInput function to the 'input' event for each date input
    dateInputs.forEach(function (input) {
        input.addEventListener('input', function () {
            validateYearInput(input);
        });
    });
});
window.blazorGoBack = function () {
    history.back();
}
// Add event listener to the search input
function searchPatients() {
    // Get the search input value
    const searchInput = document.getElementById('searchInput').value.toLowerCase();
    const searchInput2 = document.getElementById('searchInput2').value.toLowerCase();

    // Combine both search inputs
    const searchValue = searchInput || searchInput2;

    // Get all floor elements
    const floorElements = document.querySelectorAll('.bg-blue-50');

    // Loop through each floor element
    floorElements.forEach(floor => {
        // Get the floor name
        const floorName = floor.querySelector('h3').textContent.toLowerCase();

        // Get all patient elements within the floor
        const patientElements = floor.nextElementSibling.querySelectorAll('.patientfloor');

        let floorMatch = false;

        // Loop through each patient element
        patientElements.forEach(patient => {
            // Get the patient's name and room name
            const patientName = patient.querySelector('.p-1').textContent.toLowerCase();
            const roomName = patient.querySelector('h4').textContent.toLowerCase();

            // Check if the floor name, patient name, or room name includes the search input value
            if (floorName.includes(searchValue) || patientName.includes(searchValue) || roomName.includes(searchValue)) {
                // If it matches, display the patient element
                patient.style.display = 'block';
                floorMatch = true;
            } else {
                // If it doesn't match, hide the patient element
                patient.style.display = 'none';
            }
        });

        // Display the floor element if any patient matches
        if (floorMatch) {
            floor.style.display = 'block';
            floor.nextElementSibling.style.display = 'block';
        } else {
            floor.style.display = 'none';
            floor.nextElementSibling.style.display = 'none';
        }
    });
}

function searchTable() {
    // Declare variables
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("myInput");
    filter = input.value.toUpperCase();
    table = document.getElementById("myTable");
    tr = table.getElementsByTagName("tr");
    // Loop through all table rows, and hide those who don't match the search query
    for (i = 0; i < tr.length; i++) {
        // Exclude cells with specific IDs
        if (tr[i].id === "exclude") {
            continue;
        }
        else {
            td = tr[i].getElementsByTagName("td");
            for (var j = 0; j < td.length; j++) {
                if (td[j]) {
                    txtValue = td[j].textContent || td[j].innerText;
                    if (txtValue.toUpperCase().indexOf(filter) > -1) {
                        tr[i].style.display = "";
                        break; // Break the inner loop if a match is found
                    } else {
                        tr[i].style.display = "none";
                    }
                }
            }
        }
    }
}

function searchTable1() {
    // Declare variables
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("myInput1");
    filter = input.value.toUpperCase();
    table = document.getElementById("myTable1");
    tr = table.getElementsByTagName("tr");

    // Loop through all table rows, and hide those who don't match the search query
    for (i = 0; i < tr.length; i++) {
        // Exclude cells with specific IDs
        if (tr[i].id === "exclude") {
            continue;
        }
        else {
            td = tr[i].getElementsByTagName("td");
            for (var j = 0; j < td.length; j++) {
                if (td[j]) {
                    txtValue = td[j].textContent || td[j].innerText;
                    if (txtValue.trim() === "Active") {
                        break; // Break the inner loop if a match is found
                    }
                    if (txtValue.toUpperCase().indexOf(filter) === 0) {
                        tr[i].style.display = "";
                        break; // Break the inner loop if a match is found
                    } else {
                        tr[i].style.display = "none";
                    }
                }
            }
        }

    }
}

function searchTable2() {
    // Declare variables
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("myInput2");
    filter = input.value.toUpperCase();
    table = document.getElementById("myTable2");
    tr = table.getElementsByTagName("tr");

    // Loop through all table rows, and hide those who don't match the search query
    for (i = 0; i < tr.length; i++) {
        // Exclude cells with specific IDs
        if (tr[i].id === "exclude") {
            continue;
        }
        else {
            td = tr[i].getElementsByTagName("td");
            for (var j = 0; j < td.length; j++) {
                if (td[j]) {
                    txtValue = td[j].textContent || td[j].innerText;
                    if (txtValue.toUpperCase().indexOf(filter) === 0) {
                        tr[i].style.display = "";
                        break; // Break the inner loop if a match is found
                    } else {
                        tr[i].style.display = "none";
                    }
                }
            }
        }

    }
}

function searchTable3() {
    // Declare variables
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("myInput3");
    filter = input.value.toUpperCase();
    table = document.getElementById("myTable3");
    tr = table.getElementsByTagName("tr");

    // Loop through all table rows, and hide those who don't match the search query
    for (i = 0; i < tr.length; i++) {
        // Exclude cells with specific IDs
        if (tr[i].id === "exclude") {
            continue;
        }
        else {
            td = tr[i].getElementsByTagName("td");
            for (var j = 0; j < td.length; j++) {
                if (td[j]) {
                    txtValue = td[j].textContent || td[j].innerText;
                    if (txtValue.toUpperCase().indexOf(filter) === 0) {
                        tr[i].style.display = "";
                        break; // Break the inner loop if a match is found
                    } else {
                        tr[i].style.display = "none";
                    }
                }
            }
        }

    }
}
function searchTable4() {
    // Declare variables
    var input, filter, table, tr, td, i, txtValue;
    input = document.getElementById("myInput4");
    filter = input.value.toUpperCase();
    table = document.getElementById("myTable4");
    tr = table.getElementsByTagName("tr");

    // Loop through all table rows, and hide those who don't match the search query
    for (i = 0; i < tr.length; i++) {
        // Exclude cells with specific IDs
        if (tr[i].id === "exclude") {
            continue;
        }
        else {
            td = tr[i].getElementsByTagName("td");
            for (var j = 0; j < td.length; j++) {
                if (td[j]) {
                    txtValue = td[j].textContent || td[j].innerText;
                    if (txtValue.toUpperCase().indexOf(filter) === 0) {
                        tr[i].style.display = "";
                        break; // Break the inner loop if a match is found
                    } else {
                        tr[i].style.display = "none";
                    }
                }
            }
        }

    }
}

function sortTableUser() {
    var table, rows, switching, i, x, y, shouldSwitch;
    table = document.getElementById("myTable1");
    switching = true;
    while (switching) {
        switching = false;
        rows = table.rows;
        for (i = 1; i < (rows.length - 1); i++) {
            shouldSwitch = false;
            x = rows[i].getElementsByTagName("td")[columnIndex];
            y = rows[i + 1].getElementsByTagName("td")[columnIndex];
            if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                shouldSwitch = true;
                break;
            }
        }
        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
        }
    }
}

function sortTableHospital() {
    var table, rows, switching, i, x, y, shouldSwitch;
    table = document.getElementById("myTable2");
    switching = true;
    while (switching) {
        switching = false;
        rows = table.rows;
        for (i = 1; i < (rows.length - 1); i++) {
            shouldSwitch = false;
            x = rows[i].getElementsByTagName("td")[columnIndex];
            y = rows[i + 1].getElementsByTagName("td")[columnIndex];
            if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                shouldSwitch = true;
                break;
            }
        }
        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
        }
    }
}

function sortTableDesignationCode() {
    var table, rows, switching, i, x, y, shouldSwitch;
    table = document.getElementById("myTable3");
    switching = true;
    while (switching) {
        switching = false;
        rows = table.rows;
        for (i = 1; i < (rows.length - 1); i++) {
            shouldSwitch = false;
            x = rows[i].getElementsByTagName("td")[columnIndex];
            y = rows[i + 1].getElementsByTagName("td")[columnIndex];
            if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                shouldSwitch = true;
                break;
            }
        }
        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
        }
    }
}

function sortTableChargeCode() {
    var table, rows, switching, i, x, y, shouldSwitch;
    table = document.getElementById("myTable4");
    switching = true;
    while (switching) {
        switching = false;
        rows = table.rows;
        for (i = 1; i < (rows.length - 1); i++) {
            shouldSwitch = false;
            x = rows[i].getElementsByTagName("td")[columnIndex];
            y = rows[i + 1].getElementsByTagName("td")[columnIndex];
            if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                shouldSwitch = true;
                break;
            }
        }
        if (shouldSwitch) {
            rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
            switching = true;
        }
    }
}

function myFunction() {
    document.getElementById("myDropdown").classList.toggle("show");
}

// Close the dropdown if the user clicks outside of it
window.onclick = function (event) {
    if (!event.target.matches('.dropbtn')) {
        var dropdowns = document.getElementsByClassName("dropdown-content");
        for (var i = 0; i < dropdowns.length; i++) {
            var openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('show')) {
                openDropdown.classList.remove('show');
            }
        }
    }
};
window.initializeSelectPicker = (elementId) => {
    jQuery('#' + elementId).selectpicker();
};

window.clickElement = function (elementId) {
    let element = document.getElementById(elementId);
    if (element) {
        element.click();
    }
}

function filterItems(input) {
    var filter, divs, label, i, txtValue;
    filter = input;
    divs = document.getElementsByClassName("form-check");
    for (i = 0; i < divs.length; i++) {
        label = divs[i].getElementsByTagName("span")[0];
        txtValue = label.textContent || label.innerText;
        if (txtValue.toLowerCase().indexOf(filter) > -1) {
            divs[i].style.display = "";
        } else {
            divs[i].style.display = "none";
        }
    }
}
window.preventXSSPolicy = () => {
    // Regular expression to match special characters
    const specialCharPattern = /[!@#$%^&*(),.?":{}|<>_+=\[\]\\;'/`~\-]/g;

    // Regular expression to match HTML tags
    const htmlTagPattern = /<\/?[^>]+(>|$)/g;

    function preventInvalidCharacters(event) {
        const input = event.target;
        const key = event.key;

        // Allow control keys (backspace, delete, arrows, etc.)
        if (event.ctrlKey || event.altKey || event.metaKey || key.length > 1) {
            return;
        }

        // Check for special characters
        const inputValue = input.value + key;
        if (inputValue.search(specialCharPattern) !== -1 || inputValue.search(htmlTagPattern) !== -1) {
            event.preventDefault();
        }
    }

    // Sanitize input after paste
    function sanitizeInput(event) {
        const input = event.target;
        const pastedData = (event.clipboardData || window.clipboardData).getData('text');

        if (pastedData.search(specialCharPattern) !== -1 || pastedData.search(htmlTagPattern) !== -1) {
            event.preventDefault();
            input.value = input.value.replace(specialCharPattern, '').replace(htmlTagPattern, '');
        }
    }

    // Select all input fields except those with the class 'specialAllow'
    const inputs = document.querySelectorAll('input[type="text"]:not(.specialAllow)');

    inputs.forEach(input => {
        input.addEventListener('keydown', preventInvalidCharacters);
        input.addEventListener('paste', sanitizeInput);
    });
};
let lastExportedData = [];
function exportTableToExcel(filename = document.getElementById('FileName')) {
    var wb = XLSX.utils.book_new();
    var data = [];
    var MainHeading = document.getElementById('ProviderName');
    // Add main heading and subheading
    data.push([MainHeading.innerText]); // Main heading
    data.push([filename.innerText]); // Subheading
    data.push([]); // Empty row for spacing

    // Get all tables
    var tables = document.querySelectorAll('#table-container table');

    tables.forEach((mainTable, tableIndex) => {
        // Get the headers
        if (tableIndex === 0) {
            var headers = Array.from(mainTable.querySelectorAll('th')).map(th => th.innerText);
            // Remove "Census Start Date" and "Census End Date" headers
            headers = headers.filter(header => header !== "Census Start Date" && header !== "Census End Date");
            // Reorder the columns
            var newHeaders = [
                'Date & Time', 'MRN', 'Name', 'DOB', 'Code', 'Description', 'Type', 'Practitioner', 'Location'
            ];
            data.push(newHeaders);
        }

        // Iterate through each row of the main table
        for (var i = 1; i < mainTable.rows.length; i++) {
            var row = mainTable.rows[i];
            var mainRowData = [];
            var nestedTable = null;

            // Ensure the row has enough cells
            if (row.cells.length >= 7) {
                // Get main row data
                mainRowData.push(row.cells[0]?.innerText || ''); // Date & Time
                mainRowData.push(row.cells[1]?.innerText || ''); // MRN
                mainRowData.push(row.cells[2]?.innerText || ''); // Name
                mainRowData.push(row.cells[3]?.innerText || ''); // DOB

                // Look for a nested table in the 8th cell (index 7)
                if (row.cells.length > 8) {
                    nestedTable = row.cells[8].querySelector('table');
                }

                // Check if there is a nested table and add its content
                if (nestedTable) {
                    for (var j = 0; j < nestedTable.rows.length; j++) {
                        var nestedRow = nestedTable.rows[j];

                        // Prepare data for nested row
                        var nestedRowData = [];

                        // Include main row data only for the first nested row
                        if (j === 0) {
                            nestedRowData.push(...mainRowData); // Add main row data
                            nestedRowData.push(nestedRow.cells[0]?.innerText || ''); // Code
                            nestedRowData.push(nestedRow.cells[1]?.innerText || ''); // Description
                            nestedRowData.push(nestedRow.cells[2]?.innerText || ''); // Type
                            nestedRowData.push(row.cells[6]?.innerText || ''); // Practitioner
                            nestedRowData.push(row.cells[7]?.innerText.replace(/\n/g, ' ') || ''); // Location
                        } else {
                            nestedRowData.push('', '', '', ''); // Empty for main row data
                            nestedRowData.push(nestedRow.cells[0]?.innerText || ''); // Code
                            nestedRowData.push(nestedRow.cells[1]?.innerText || ''); // Description
                            nestedRowData.push(nestedRow.cells[2]?.innerText || ''); // Type
                            nestedRowData.push('', ''); // Empty for Practitioner and Location
                        }

                        // Push the data to the main array
                        data.push(nestedRowData);
                    }
                } else {
                    // If no nested table, just push the main row data with empty cells for Code, Description, Type
                    mainRowData.push('', '', ''); // Add empty cells for Code, Description, Type
                    mainRowData.push(row.cells[6]?.innerText || ''); // Practitioner
                    mainRowData.push(row.cells[7]?.innerText.replace(/\n/g, ' ') || ''); // Location
                    data.push(mainRowData);
                }
            }
        }
        // Add an empty row to create a gap between tables, but only if it's not the last table
        // if (tableIndex < tables.length - 1) {
        //     data.push([]);
        // }
    });

    // Create a worksheet from the data
    var ws = XLSX.utils.aoa_to_sheet(data);

    // Apply styles to the main heading and subheading
    ws['A1'].s = { font: { sz: 26, bold: true }, alignment: { horizontal: "center" } };
    ws['A2'].s = { font: { sz: 22, bold: true }, alignment: { horizontal: "center" } };

    // Merge cells for the main heading and subheading to center them
    const mergeRange1 = { s: { r: 0, c: 0 }, e: { r: 0, c: 8 } }; // Merge A1 to I1
    const mergeRange2 = { s: { r: 1, c: 0 }, e: { r: 1, c: 8 } }; // Merge A2 to I2
    if (!ws['!merges']) ws['!merges'] = [];
    ws['!merges'].push(mergeRange1);
    ws['!merges'].push(mergeRange2);

    // Apply bold styling to the header row
    const headerRange = XLSX.utils.decode_range(ws['!ref']);
    for (let C = headerRange.s.c; C <= headerRange.e.c; ++C) {
        const cellAddress = XLSX.utils.encode_cell({ r: 3, c: C }); // Adjusted row number for headers
        if (!ws[cellAddress]) continue;
        if (!ws[cellAddress].s) ws[cellAddress].s = {};
        ws[cellAddress].s.font = { bold: true };
    }

    // Apply grey and white striped pattern and borders to the rows
    const range = XLSX.utils.decode_range(ws['!ref']);
    for (let R = 4; R <= range.e.r; ++R) { // Start from row 4 to skip headings
        for (let C = range.s.c; C <= range.e.c; ++C) {
            const cellAddress = XLSX.utils.encode_cell({ r: R, c: C });
            if (!ws[cellAddress]) continue;
            if (!ws[cellAddress].s) ws[cellAddress].s = {};
            ws[cellAddress].s.fill = { fgColor: { rgb: (R % 2 === 0) ? "E9E9E9" : "FFFFFF" } };
            ws[cellAddress].s.border = {
                top: { style: "thin", color: { rgb: "000000" } },
                bottom: { style: "thin", color: { rgb: "000000" } },
                left: { style: "thin", color: { rgb: "000000" } },
                right: { style: "thin", color: { rgb: "000000" } }
            };
        }
    }

    lastExportedData = data; // Store the exported data for later use
    // Add the worksheet to the workbook
    XLSX.utils.book_append_sheet(wb, ws, "Patient Visits");

    // Export the workbook
    XLSX.writeFile(wb, filename.innerText + '.xlsx');
}
// PDF export function
function exportCombinedTableToPDF() {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF('l', 'mm', 'a3');

    // Main heading and subheading
    const mainHeading = document.getElementById('ProviderName').innerText;
    const subHeading = document.getElementById('FileName').innerText;

    // Add main heading
    doc.setFontSize(26);
    doc.text(mainHeading, 20, 30);

    // Center the subheading
    doc.setFontSize(22);
    const pageWidth = doc.internal.pageSize.getWidth();
    const textWidth = doc.getTextWidth(subHeading);
    const textX = (pageWidth - textWidth) / 2;
    doc.text(subHeading, textX, 50);

    // Initialize arrays for table data
    const combinedTableHead = [];
    const combinedTableBody = [];

    // Get all tables within the container
    const tables = document.querySelectorAll('#table-container table');

    tables.forEach((table) => {
        const tableHead = Array.from(table.querySelectorAll('th')).map(th => th.innerText);

        // Modify the header to replace "Census Start Date" and "Census End Date" with "Code", "Description", "Type"
        if (combinedTableHead.length === 0) {
            tableHead.splice(4, 2, "Code", "Description", "Type");
            combinedTableHead.push(...tableHead.slice(0, 7)); // Only include up to "Location"
            combinedTableHead.push("Practitioner", "Location"); // Add Practitioner and Location
        }

        // Extract data from the main table
        const mainTable = table.querySelector('tbody');
        Array.from(mainTable.rows).forEach(row => {
            const mainRowData = [];
            let nestedTable = null;

            // Ensure the row has enough cells
            if (row.cells.length >= 8) {
                // Get main row data
                mainRowData.push(row.cells[0]?.innerText || ''); // Date & Time
                mainRowData.push(row.cells[1]?.innerText || ''); // MRN
                mainRowData.push(row.cells[2]?.innerText || ''); // Name
                mainRowData.push(row.cells[3]?.innerText || ''); // DOB

                // Check if the cell for the nested table exists
                if (row.cells.length > 8) {
                    nestedTable = row.cells[8].querySelector('table');
                }

                // Check if there is a nested table
                if (nestedTable) {
                    Array.from(nestedTable.rows).forEach((nestedRow, j) => {
                        const nestedRowData = [];

                        // Include main row data only for the first nested row
                        if (j === 0) {
                            nestedRowData.push(...mainRowData);
                            nestedRowData.push(nestedRow.cells[0]?.innerText || ''); // Code
                            nestedRowData.push(nestedRow.cells[1]?.innerText || ''); // Description
                            nestedRowData.push(nestedRow.cells[2]?.innerText || ''); // Type
                            nestedRowData.push(row.cells[6]?.innerText || ''); // Practitioner
                            nestedRowData.push(row.cells[7]?.innerText.replace(/\n/g, ' ') || ''); // Location
                        } else {
                            nestedRowData.push('', '', '', ''); // Empty for main row data
                            nestedRowData.push(nestedRow.cells[0]?.innerText || ''); // Code
                            nestedRowData.push(nestedRow.cells[1]?.innerText || ''); // Description
                            nestedRowData.push(nestedRow.cells[2]?.innerText || ''); // Type
                            nestedRowData.push('', ''); // Empty for Practitioner and Location
                        }

                        // Push the data to the combined body
                        combinedTableBody.push(nestedRowData);
                    });
                } else {
                    // If no nested table, just push the main row data with empty cells for Code, Description, Type
                    mainRowData.push('', '', ''); // Add empty cells for Code, Description, Type
                    mainRowData.push(row.cells[6]?.innerText || ''); // Practitioner
                    mainRowData.push(row.cells[7]?.innerText.replace(/\n/g, ' ') || ''); // Location
                    combinedTableBody.push(mainRowData);
                }
            }
        });
    });

    // Create the table using autoTable
    doc.autoTable({
        head: [combinedTableHead], // Combined header row
        body: combinedTableBody, // Combined data rows
        startY: 60, // Position table below headings
        theme: 'striped',
        styles: { cellPadding: 2, fontSize: 10, overflow: 'linebreak' },
        headStyles: { fillColor: [22, 160, 133] }, // Customize header color
        alternateRowStyles: { fillColor: [240, 240, 240] }, // Alternating row colors
    });

    // Save the PDF
    doc.save(subHeading + '.pdf');
}
function filterVisits() {
    // Get the search input value
    var input = document.getElementById('searchInput');
    var filter = input.value.toUpperCase();

    // Get all visit containers
    var visitContainers = document.querySelectorAll('.visit-container');

    // Loop through all visit containers and hide those that don't match the search query
    visitContainers.forEach(function (container) {
        var patientName = container.querySelector('h5').textContent || container.querySelector('h5').innerText;
        if (patientName.toUpperCase().indexOf(filter) > -1) {
            container.style.display = "";
        } else {
            container.style.display = "none";
        }
    });
}

function validateYearInput(input) {
    let value = input.value;
    // Split the date into parts
    let parts = value.split("-");
    if (parts.length > 0) {
        // Check if the year has more than 4 digits
        if (parts[0].length > 4) {
            // Limit the year to 4 digits
            parts[0] = parts[0].slice(0, 4);
        }
        // Reassemble the date and set the value
        input.value = parts.join("-");
    }
}

function restrictYearInputById(id) {
    const dateInput = document.getElementById(id);
    if (dateInput) {
        let value = dateInput.value;
        // Split the date into parts
        let parts = value.split("-");
        if (parts.length > 0) {
            // Check if the year has more than 4 digits
            if (parts[0].length > 4) {
                // Limit the year to 4 digits
                parts[0] = parts[0].slice(0, 4);
            }
            // Reassemble the date and set the value
            dateInput.value = parts.join("-");
        }
    }
}
window.preventNonNumericAndFocus = function (monthElement, dayElement, yearElement) {
    // Validate Month input
    monthElement.value = monthElement.value.replace(/[^0-9]/g, ''); // Remove non-numeric characters

    // If more than two characters are entered for Month, truncate
    if (monthElement.value.length > 2) {
        monthElement.value = monthElement.value.slice(0, 2);
    }

    // Only start validating the Month input after it has exactly two digits
    if (monthElement.value.length === 2) {
        const monthValue = parseInt(monthElement.value, 10);
        // If Month is out of range (1-12), reset it to empty
        if (monthValue < 1 || monthValue > 12) {
            monthElement.value = '';
        } else {
            // If valid, move focus to the Day input field only if Day is empty
            if (dayElement.value.length === 0) {
                dayElement.focus();
            }
        }
    }

    // Validate Day input
    dayElement.value = dayElement.value.replace(/[^0-9]/g, ''); // Remove non-numeric characters

    // If more than two characters are entered for Day, truncate
    if (dayElement.value.length > 2) {
        dayElement.value = dayElement.value.slice(0, 2);
    }

    // Only start validating the Day input after it has exactly two digits
    if (dayElement.value.length === 2) {
        const dayValue = parseInt(dayElement.value, 10);
        // If Day is out of range (1-31), reset it to empty
        if (dayValue < 1 || dayValue > 31) {
            dayElement.value = '';
        } else {
            // If valid, move focus to the Year input field only if Year is empty
            if (yearElement.value.length === 0) {
                yearElement.focus();
            }
        }
    }

    // Validate Year input
    yearElement.value = yearElement.value.replace(/[^0-9]/g, ''); // Remove non-numeric characters

    // If more than four characters are entered for Year, truncate
    if (yearElement.value.length > 4) {
        yearElement.value = yearElement.value.slice(0, 4);
    }

    // Only start validating the Year input after it has exactly four digits
    if (yearElement.value.length === 4) {
        const yearValue = parseInt(yearElement.value, 10);
        // If Year is out of range (1900-2099), reset it to empty
        if (yearValue < 1900 || yearValue > 2099) {
            yearElement.value = '';
        }
    }
};
window.validateDateFormat = (dateString) => {
    // Check if the date string is valid (ISO 8601 format)
    if (!dateString) {
        return false; // Invalid date string
    }

    // Parse the ISO 8601 date string to a JavaScript Date object
    const date = new Date(dateString);

    // Check if the Date object is valid by comparing the year, month, and day
    const day = date.getDate();
    const month = date.getMonth() + 1; // JavaScript months are 0-indexed
    const year = date.getFullYear();

    // Format the date as "DD/MM/YYYY"
    const formattedDate = `${String(day).padStart(2, '0')}/${String(month).padStart(2, '0')}/${year}`;

    // Define regex pattern for validating the full date format: "DD/MM/YYYY"
    const datePattern = /^(\d{2})\/(\d{2})\/(\d{4})$/;
    const match = formattedDate.match(datePattern);

    if (!match) {
        return false; // Invalid format
    }

    // Check if the date is valid by comparing it with the newly created Date object
    const testDate = new Date(year, month - 1, day); // months are 0-indexed
    if (testDate.getDate() !== day || testDate.getMonth() !== month - 1 || testDate.getFullYear() !== year) {
        return false; // Invalid date (e.g., 31st of February or invalid date)
    }

    return true; // Valid date
};
