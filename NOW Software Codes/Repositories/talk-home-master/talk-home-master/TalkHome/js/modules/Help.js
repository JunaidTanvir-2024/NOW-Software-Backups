/**
 * Contains methods for the Help section.
 * 
 * @author micheled
 * Created on 16.07.2017
 * 
 */
'use strict';
var Help = function () {

    /*******************
     * Private methods *
     ******************/
    
    /**
     * Finds all h2 tags in the text and populates `In this section` with links
     * 
     * @param jQueryObject The text to scan.
     * 
     */
    function createSectionLinks (section) {
        $(section).find('h2').each(function () {
            $('.first.section').find('.headings').append('<a class="section-link" href=""></a>');
            
            var lastLink = $('.first.section .headings').find('a:last-child');
            var slug = $(this).text().toLowerCase().replace(/ /g, '-').replace(/[^\w-]+/g, '');

            lastLink.text($(this).text()).attr('href', '#' + slug);
            $(this).attr('id', slug);
        });
    }

    /*************
     * Ui events *
     *************/
    createSectionLinks($('.help-page').find('.middle.section .inner'));

}();
