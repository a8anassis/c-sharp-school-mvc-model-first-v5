function confirmDelete(userId) {
    if (confirm("Are you sure you want to delete this student?")) {
        window.location.href = "/Students/" + studentId + "/Delete"
    }
}