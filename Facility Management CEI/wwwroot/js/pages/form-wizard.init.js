/*
Template Name: Skote - Responsive Bootstrap 4 Admin Dashboard
Author: Themesbrand
Version: 2.0
Website: https://themesbrand.com/
Contact: themesbrand@gmail.com
File: Form wizard Js File
*/

$(document).ready(function() {
    $('#basic-pills-wizard').bootstrapWizard({
        'tabClass': 'nav nav-pills nav-justified'
    });

    $('#progrss-wizard').bootstrapWizard({onTabShow: function(tab, navigation, index) {
        var $total = navigation.find('li').length;
        var $current = index+1;
        var $percent = ($current/$total) * 100;
        $('#progrss-wizard').find('.progress-bar').css({width:$percent+'%'});
    }});

    
});
