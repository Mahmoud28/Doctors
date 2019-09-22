var notification;
var container = document.getElementById('notification-container');
var visible = false;
var queue = [];
window.onload = function () {
    container = document.getElementById('notification-container');
    //showNotification('success', 'Success!', 'Your request was completed successfully.');
};
function createNotification() {
    notification = document.createElement('div');
    var btn = document.createElement('button');
    var title = document.createElement('div');
    var msg = document.createElement('div');
    btn.className = 'notification-close';
    title.className = 'notification-title';
    msg.className = 'notification-message';
    btn.addEventListener('click', hideNotification, false);
    notification.addEventListener('animationend', hideNotification, false);
    notification.addEventListener('webkitAnimationEnd', hideNotification, false);
    notification.appendChild(btn);
    notification.appendChild(title);
    notification.appendChild(msg);
}

function updateNotification(type, title, message) {
    notification.className = 'notification notification-' + type;
    notification.querySelector('.notification-title').innerHTML = title;
    notification.querySelector('.notification-message').innerHTML = message;
}

function showNotification(type, title, message) {
    if (visible) {
        queue.push([type, title, message]);
        return;
    }
    if (!notification) {
        createNotification();
    }
    updateNotification(type, title, message);
    container.append(notification);
    visible = true;
}

function hideNotification() {
    if (visible) {
        visible = false;
        container.removeChild(notification);
        if (queue.length) {
            showNotification.apply(null, queue.shift());
        }
    }
}

//for testing add html code to your page
//<button type="button" class="btn btn-danger" onclick="showNotification('error', 'خطأ !', 'حدث خطأ بالنظام')">
//    error
//                </button>
//    <button type="button" class="btn btn-info" onclick="showNotification('warning', 'تحذير!', 'الخطة غير مكتملة')">
//        warning
//                </button>
//    <button type="button" class="btn btn-primary" onclick="showNotification('info', '!  ', 'اضغط حفظ واستمرار لمتابعة التعديل')">
//        info
//                </button>
//    <button type="button" class="btn btn-success" onclick="showNotification('success', 'تم الحفظ !', 'تم حفظ البيانات بنجاح')">
//        success
//                </button>