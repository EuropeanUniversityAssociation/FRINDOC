var currentIndex = 0;
function switchPanel(index, selector) {
    displayPanel(selector, currentIndex <= index ? 'left' : 'right');
    currentIndex = index;
    $("#navigation li").removeClass('active');
    $("#navigation li#link-" + index).addClass('active');

    if (index == 4) { // results panel
        window.setTimeout(function () {
            $(".notification-top").slideDown(300);
            $(".notification-top").click(function () { $(this).slideUp(200); });
        }, 2000);
        
    }
}

var currentSelector = '';
function displayPanel(selector, direction) {
    if (currentSelector != selector) {
        direction = (direction || 'left')
        var inverse = (direction == 'left') ? 'right' : 'left';
        if ($(".panel:visible").length > 0) {
            $(".panel:visible").hide('slide', { direction: direction }, 200, function () {
                $(selector + ':hidden').show('slide', { direction: inverse }, 200, function () {
                    $("input:first", selector).focus();
                });
            });
        } else {
            $(selector + ':hidden').show('slide', { direction: inverse }, 0, function () {
                $("input:first", selector).focus();
            });
        }
        currentSelector = selector;
    }
}

function setPageTitle(university)
{
    var year = (new Date).getFullYear();
    var title = 'FRINDOC Self-Evaluation Report, '+university+', '+year;
    document.title = title;
    $("#pagetitle").html(title);
    $("#pagetitle-print").html(university);
}

$('#loader').bind("ajaxSend", function () {
    $(this).show();
}).bind("ajaxComplete", function () {
    $(this).hide();
});

// Initialisation

//displayPanel('#panelIntroduction');

// Compatibility code for older browsers: 
if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (obj, start) {
        for (var i = (start || 0), j = this.length; i < j; i++) {
            if (this[i] === obj) { return i; }
        }
        return -1;
    }
}