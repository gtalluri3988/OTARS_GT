/**
 * Layout initialization script - CSP compliant
 * This file contains initialization code that was previously inline in _Layout.cshtml
 */

// Global configuration object to store URLs and settings
window.MTAoarsConfig = window.MTAoarsConfig || {};

// Function to initialize the layout when DOM is ready
$(function() {
    // Initialize Kendo Menu if element exists
    if ($("#page-menu").length > 0) {
        $("#page-menu").kendoMenu();
    }
});

// Function to set configuration URLs (called from layout with nonce)
window.setMTAoarsConfig = function(config) {
    window.MTAoarsConfig = config;
};
