


$(document).ready(function () {

    $('.navbar-burger').click(function () {
        $('#navBarBurger, .navbar-burger').toggleClass('is-active');
    });

    $('input[type="file"]').change(function (e) {
        var fileName = e.target.files[0].name;
        const fileLabel = $('#file-upload .file-name');
        fileLabel.text(fileName);
    });


    $('select[data-type="tags"]').each(function (el) {

        new BulmaTagsInput($(this)[0], {
            allowDuplicates: false,
            caseSensitive: false,
            clearSelectionOnTyping: false,
            closeDropdownOnItemSelect: true,
            delimiter: ',',
            freeInput: false,
            highlightDuplicate: true,
            highlightMatchesString: true,
            itemValue: undefined,
            itemText: undefined,
            maxTags: undefined,
            maxChars: undefined,
            minChars: 1,
            noResultsLabel: 'No results found',
            placeholder: 'Choose genres',
            removable: true,
            searchMinChars: 3,
            searchOn: 'text',
            selectable: false,
            tagClass: 'is-rounded',
            trim: true
            //source: async function (value) {
            //    // Value equal input value
            //    // We can then use it to request data from external API
            //    //return await fetch("/Showpiece/Genre/List/")
            //    //    .then(function (response) {
            //    //        return response.json();
            //    //    });
            //}
        });
    });

    var name = "";
    var disabled = false;
    var rating = 0;
    if ($(".mtgdm_xyz").length > 0) {
        name = $(".mtgdm_xyz")[0].id;
        const formName = '#form_' + name;
        const form = $(formName);
        const formData = new FormData(form[0]);
        if (formData.get('disabled') === "True") {
            disabled = true;
        }
        else {
            disbabled = false;
        }
        //disabled = formData.get('disabled');
        rating = formData.get('rating');
    }

    $(".mtgdm_xyz").starRating({
        
        starSize: 25,
        initialRating: rating,
        useGradient: false,
        strokeWidth: 0,
        emptyColor: 'lightgray',
        hoverColor: 'salmon',
        activeColor: 'crimson',
        disableAfterRate: false,
        readOnly: !!disabled,
        callback: function (currentRating, $el) {
            // make a server call here
            const formName = '#form_' + $el[0].id;
            var form = $(formName);
            var formData = new FormData(form[0]);
            formData.set("rating", currentRating);
            var url = form.attr('action');
            console.log(url);
            $.ajax({
                type: "POST",
                url: url,
                data: formData,
                processData: false,
                contentType: false,
                success: function (data) {
                    console.log("Success from AJAX");
                },
                error: function (data) {
                    console.log("Error response from /Rating", data);
                }
            });
        }
    });

    $('.block-ui').click(function () {
        $.blockUI({
            css: {
                border: 'none',
                padding: 'calc(.5em - 1px)',
                backgroundColor: '#375a7f',
                'border-radius': '.4em',
                opacity: 1,
                color: '#fff'
            }
        });
    });
    
    $('#cookieConsent > div > div > div > button').click(function () {
        document.cookie = this.dataset.cookieString;
        $('#cookieConsent').hide();
    });

});
