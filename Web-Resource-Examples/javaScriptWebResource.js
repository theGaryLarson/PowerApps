/**
 * The JavaScript code below defines several functions that handle retrieving and manipulating data from a Dynamics 365 CRM
 * system, specifically related to university and student records.
 * @param contentWindow - The `contentWindow` parameter is the window object of the web resource that is being loaded. It
 * allows you to access and manipulate the content of the web resource. In this code, it is used to pass the `XRM` object
 * and the `lookUpValue` object to the web resource
 */
const lookUpValue = {};

/**
 * formContext variable to store reference to form context.
 *
 * Declared as global variable and initialized to null. Will be set to
 * the form context object when the form loads.
 *
 * @type {Object|null}
 */
let formContext = null;

/**
 * executionContext variable to store the form execution context.
 *
 * Declared globally and initialized to null. Will be set to the
 * execution context object when the form loads.
 *
 * @type {Object|null}
 */
let executionContext = null;

/**
 * The function assigns values to properties of the contentWindow object and then calls the fetchStudents function with the
 * studentData parameter.
 * @param contentWindow - The contentWindow parameter is a reference to the window object of the content that is loaded in
 * an iframe or a popup window. It allows you to access and manipulate the content within that window.
 */
function handleWindowContent(contentWindow) {
    contentWindow.lookupData = lookUpValue;
    console.log('lookUpValue', lookUpValue);
    console.log('CNTWIN:', contentWindow);
    contentWindow.fetchStudents(studentData);
}
/**
 * The function `handleError` opens an error dialog in a CRM system and logs the error message.
 * @param error - The error object that contains information about the error that occurred. It typically has a "message"
 * property that provides a description of the error.
 */
function handleError(error) {
    Xrm.Navigation.openErrorDialog({ message: error.message }).then(
        success => console.log(error.message),
        error => console.log('An error occurred while opening the error dialog:', error.message)
    );
}

/**
 * The function "handleStudentRecords" logs the student data, retrieves an HTML control, and then handles the window
 * content.
 * @param students - The `students` parameter is an array of student records. Each student record is an object that
 * contains information about a student, such as their name, age, and grade.
 */
function handleStudentRecords(students) {
    studentData = students;
    console.log('STUDENT DATA ONLOAD:', students);
    const htmlControl = formContext.getControl('WebResource_Student');
    console.log('HTMLCONTROL:', htmlControl);
    htmlControl.getContentWindow().then(handleWindowContent, handleError);
}

/**
 * The function retrieves student records associated with a specific university.
 * @param universityRows - The `universityRows` parameter is an object that contains information about university records.
 * It is expected to have a property called `entities` which is an array of university entities. Each university entity
 * should have a property called `in23gl_universityid` which represents the university ID.
 */
function handleUniversityRecords(universityRows) {
    lookUpValue.universityEntity = universityRows.entities[0];
    const universityId = universityRows.entities[0].in23gl_universityid;
    const idFilter = `?$filter=_in23gl_university_value eq '${universityId}'`;
    Xrm.WebApi.retrieveMultipleRecords('in23gl_student', idFilter).then(handleStudentRecords, handleError);
}

/**
 * The function retrieves the "in23gl_university" entity based on the value of the
 * "in23gl_universityname" attribute selected in the University Main Form and handles the retrieved records.
 * @param context - The `context` parameter is the execution context of the form. It provides access to various methods and
 * properties related to the form and its data. In this case, it is used to get the form context using the
 * `getFormContext()` method.
 */
function onLoad(context) {
    executionContext = context;
    formContext = context.getFormContext();
    console.log('formContextOnLoad:', formContext);

    const universityName = formContext.getAttribute('in23gl_universityname').getValue();
    const filter = `?$filter=in23gl_universityname eq '${universityName}'`;
    Xrm.WebApi.retrieveMultipleRecords('in23gl_university', filter).then(handleUniversityRecords, handleError);
}
