

window.onscroll = function () {
    scrollFunction()
};
$("#myBtn").click(function () {
    $('html, body').animate({
        scrollTop: 0
    }, 1000);
    return false;
});

function scrollFunction() {
    if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
        document.getElementById("myBtn").style.display = "block";
    } else {
        document.getElementById("myBtn").style.display = "none";
    }
}
