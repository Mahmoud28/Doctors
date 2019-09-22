var translations = {
    seagull: {
        buttons: {
            BACK: "رجوع",
            SAVE: "حفظ",
            SAVE_CONTINUE: "حفظ و استمرار",
            SAVE_EDITPLAN: "حفظ الخطة والبدء بادخال تفاصيلها",
            SAVE_SENDPLAN: "حفظ وارسال الخطة الى مدراء المديريات",
            Accept_senttomanagement: "ارسال الى مدراء الادارات",
            SAVE_SENDPLANTOLiaisonOfficer: "موافقة وارسال الى ضابط الارتباط",
            Show_Plan : "عرض الخطة",
            Project_MainPage: "الصفحة الرئيسية للمشاريع",
            SAVE_DRAFT: "حفظ كمسودة",
            UNDO: "تراجع",
            ADD: "اضافة جديد",
            EDIT: "تعديل",
            DELETE: "حذف",
            REFRESH: "تحديث البيانات",
            CANCEL: "الغاء",
            SUBMIT: "ارسال",
            NEXT: "الصفحة التالية",
            PREVIOUS: "الصفحة السابقة",
            FIRST: "الصفحة الاولى",
            LAST: "الصفحة الاخيرة"
        },
        validation: {
            REQUIRED: 'الحقل {{param}} مطلوب',
            MINLENGTH: "يجب ادخال {{param}} حرف على الاقل",
            MAXLENGTH: "لا يمكن ادخال اكثر من {{param}} حرف",
            MIN: "يجب ان تكون القيمة المدخلة تساوي او اكبر من ",
            MAX: "يجب ان تكون القيمة المدخلة تساوي او اقل من ",
            EMAIL: "الصيغة المدخلة للبريد الالكترونة خاطئة",
            DATE: "الصيغة المدخلة للتاريخ خاطئة",
            PATTERN: "مرحبا"
        },
        operators: {
            NQ: 'غير مطابق',
            EQ: 'مطابق',
            IN: 'يحتوى على جزء من النص'
        },
        messages: {
            DELETE_CONFIRM: "تأكيد الحذف",
            DELETE_CONFIRM_MSG: "هل انت متأكد بأنك تريد حذف هذا السطر؟",
            NO_DATA: "لا يوجد بيانات.",
            LOADING: "جاري تحميل البيانات ..."
        },
        labels: {
            PAGE: 'الصفحة',
            PAGE_ROWS: 'عدد الاسطر ',
            OF: 'من',
            TOTAL_ROWS: 'عدد المدخلات:',
            OPERATION: 'الاجراء'
        }
    }
};
var ListOfValues = {
    Quarters: {
        1: "Q1",
        2: "Q2",
        3: "Q3",
        4: "Q4",
    }
}
angular.extend(angular, { translations: translations, ListOfValues: ListOfValues });
