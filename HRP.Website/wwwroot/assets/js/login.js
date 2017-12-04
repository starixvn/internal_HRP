$("form").submit(function( event ) {
    event.preventDefault();
    $('input[type=text]').each(function(index, element) {
        var inputValue = $(element).val();
        if (!inputValue)
        {
            $(this).parent().effect('shake');
        }
    });

    event.currentTarget.submit();
  });