/**
 * CSP-Compliant Event Handling System
 * Replaces inline onclick handlers with proper event delegation
 */

(function($) {
    'use strict';

    // Main event handler namespace
    window.MTAoarsEvents = window.MTAoarsEvents || {};

    // Initialize when DOM is ready
    $(document).ready(function() {
        initializeEventHandlers();
    });

    function initializeEventHandlers() {
        // Grid button handlers
        setupGridButtonHandlers();
        
        // Navigation handlers
        setupNavigationHandlers();
        
        // Form handlers
        setupFormHandlers();
        
        // File download handlers
        setupFileHandlers();
        
        // Administrative handlers
        setupAdministrativeHandlers();
    }

    // Grid button event handlers
    function setupGridButtonHandlers() {
        // Handle edit buttons
        $(document).on('click', '[data-action="edit"]', function(e) {
            e.preventDefault();
            if (typeof window.onEditClick === 'function') {
                window.onEditClick();
            }
        });

        // Handle add buttons  
        $(document).on('click', '[data-action="add"]', function(e) {
            e.preventDefault();
            if (typeof window.onAddClick === 'function') {
                window.onAddClick();
            }
        });

        // Handle submit buttons
        $(document).on('click', '[data-action="submit"]', function(e) {
            e.preventDefault();
            if (typeof window.submitDetails === 'function') {
                window.submitDetails();
            }
        });

        // Handle save buttons
        $(document).on('click', '[data-action="save"]', function(e) {
            e.preventDefault();
            if (typeof window.saveDetails === 'function') {
                window.saveDetails();
            }
        });

        // Handle terminate buttons
        $(document).on('click', '[data-action="terminate"]', function(e) {
            e.preventDefault();
            if (typeof window.terminate === 'function') {
                window.terminate();
            }
        });

        // Handle copy directors
        $(document).on('click', '[data-action="copy-directors"]', function(e) {
            e.preventDefault();
            if (typeof window.copyDirectors === 'function') {
                window.copyDirectors();
            }
        });

        // Handle export to CSV
        $(document).on('click', '[data-action="export-csv"]', function(e) {
            e.preventDefault();
            if (typeof window.exportToCsv === 'function') {
                window.exportToCsv();
            }
        });

        // Handle cancel changes
        $(document).on('click', '[data-action="cancel-changes"]', function(e) {
            e.preventDefault();
            if (typeof window.cancelChanges === 'function') {
                window.cancelChanges();
            }
        });
    }

    // File download handlers
    function setupFileHandlers() {
        // Handle download buttons with file ID
        $(document).on('click', '[data-action="download"]', function(e) {
            e.preventDefault();
            var fileId = $(this).data('file-id');
            if (fileId && typeof window.downloadFile === 'function') {
                window.downloadFile(fileId);
            }
        });

        // Handle generic download with URL
        $(document).on('click', '[data-action="download-url"]', function(e) {
            e.preventDefault();
            var url = $(this).data('download-url');
            if (url && typeof window.download === 'function') {
                window.download(url);
            }
        });
    }

    // Navigation handlers
    function setupNavigationHandlers() {
        // Handle logout with custom logic
        $(document).on('click', '[data-action="logout"]', function(e) {
            if (typeof window.MTAoarsLogout === 'function') {
                e.preventDefault();
                window.MTAoarsLogout();
                return false;
            }
        });
    }

    // Administrative handlers
    function setupAdministrativeHandlers() {
        // Handle show details buttons
        $(document).on('click', '[data-action="show-details"]', function(e) {
            e.preventDefault();
            var agencyId = $(this).data('agency-id');
            var agencyType = $(this).data('agency-type');
            var principalId = $(this).data('principal-id');
            var memberId = $(this).data('member-id');
            
            if (typeof window.showDetails === 'function') {
                window.showDetails(agencyId, agencyType, principalId, memberId);
            }
        });
    }

    // Form handlers
    function setupFormHandlers() {
        // Add any form-specific event handlers here
        // For example, validation, submission, etc.
    }

    // Utility function to convert inline handlers to data attributes
    MTAoarsEvents.convertInlineHandler = function(element, action, data) {
        var $el = $(element);
        $el.attr('data-action', action);
        
        if (data) {
            for (var key in data) {
                $el.attr('data-' + key, data[key]);
            }
        }
        
        // Remove any existing onclick attribute
        $el.removeAttr('onclick');
    };

    // Export for global use
    window.MTAoarsEvents = MTAoarsEvents;

})(jQuery);
