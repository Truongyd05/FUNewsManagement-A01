// Popup dialogs: forms are loaded into a Bootstrap modal and submitted with AJAX.
$(function () {
    const modalEl = document.getElementById('appModal');
    if (!modalEl) return;
    const modal = new bootstrap.Modal(modalEl);
    const $content = $(modalEl).find('.modal-content');

    function show(html) {
        $content.html(html);
        $.validator.unobtrusive.parse($content);
    }

    $(document).on('click', '[data-modal-url]', function (e) {
        e.preventDefault();
        $.get($(this).data('modal-url'))
            .done(function (html) { show(html); modal.show(); })
            .fail(function () { alert('Unable to load the requested item.'); });
    });

    $(modalEl).on('submit', 'form[data-ajax-form]', function (e) {
        e.preventDefault();
        const $form = $(this);
        if ($form.valid && !$form.valid()) return;
        $.post($form.attr('action'), $form.serialize())
            .done(function (res) {
                if (res && res.success) {
                    modal.hide();
                    location.reload();
                } else {
                    show(res);
                }
            })
            .fail(function () { alert('The request failed. Please try again.'); });
    });
});
