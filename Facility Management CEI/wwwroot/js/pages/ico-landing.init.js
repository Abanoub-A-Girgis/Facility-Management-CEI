/*
Template Name: Skote - Responsive Bootstrap 4 Admin Dashboard
Author: Themesbrand
Version: 2.0
Website: https://themesbrand.com/
Contact: themesbrand@gmail.com
File: ico landing Init Js File
*/

// Sticky nav

$(window).scroll(function() {
    var scroll = $(window).scrollTop();

    if (scroll >= 50) {
        $(".sticky").addClass("nav-sticky");
    } else {
        $(".sticky").removeClass("nav-sticky");
    }
});


// Smoothlink

$('.navbar-nav a').on('click', function(event) {
    var $anchor = $(this);
    $('html, body').stop().animate({
        scrollTop: $($anchor.attr('href')).offset().top - 0
    }, 1500, 'easeInOutExpo');
    event.preventDefault();
});


// Countdown

$('[data-countdown]').each(function () {
    var $this = $(this), finalDate = $(this).data('countdown');
    $this.countdown(finalDate, function (event) {
        $(this).html(event.strftime(''
        + '<div class="coming-box">%D <span>Days</span></div> '
        + '<div class="coming-box">%H <span>Hours</span></div> '
        + '<div class="coming-box">%M <span>Minutes</span></div> '
        + '<div class="coming-box">%S <span>Seconds</span></div> '));
    });
});


// Clients carousel

$('#clients-carousel, #team-carousel').owlCarousel({
    items: 1,
    loop:false,
    margin:24,
    nav: false,
    dots: false,
    responsive:{
        576:{
            items:2
        },

        768:{
            items:3
        },
        
        992:{
            items:4
        },
    }
});


// Timeline carousel

$('#timeline-carousel').owlCarousel({
    items: 1,
    loop: false,
    margin:0,
    nav: true,
    navText : ["<i class='mdi mdi-chevron-left'></i>","<i class='mdi mdi-chevron-right'></i>"],
    dots: false,
    responsive:{
        576:{
            items:2
        },

        768:{
            items:3
        },

        992:{
            items:4
        },
    }
});