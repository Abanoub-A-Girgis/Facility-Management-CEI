/*
Template Name: Skote - Responsive Bootstrap 4 Admin Dashboard
Author: Themesbrand
Version: 2.0
Website: https://themesbrand.com/
Contact: themesbrand@gmail.com
File: Session Timeout Js File
*/

$.sessionTimeout({
	keepAliveUrl: '\',
	logoutButton:'Logout',
	logoutUrl: '\',
	redirUrl: '\',
	warnAfter: 3000,
	redirAfter: 30000,
	countdownMessage: 'Redirecting in {timer} seconds.'
});