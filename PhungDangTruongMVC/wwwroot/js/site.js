// FU News Management — UI behaviours.
// Popup dialogs: forms are loaded into a Bootstrap modal and submitted with AJAX.
$(function () {
    const modalEl = document.getElementById('appModal');
    if (modalEl) {
        const modal = new bootstrap.Modal(modalEl);
        const $content = $(modalEl).find('.modal-content');

        function show(html) {
            $content.html(html);
            $.validator.unobtrusive.parse($content);
        }

        $(document).on('click', '[data-modal-url]', function (e) {
            e.preventDefault();
            const $btn = $(this).prop('disabled', true);
            $.get($(this).data('modal-url'))
                .done(function (html) { show(html); modal.show(); })
                .fail(function () { alert('Unable to load the requested item.'); })
                .always(function () { $btn.prop('disabled', false); });
        });

        $(modalEl).on('submit', 'form[data-ajax-form]', function (e) {
            e.preventDefault();
            const $form = $(this);
            if ($form.valid && !$form.valid()) return;
            const $submit = $form.find('button[type="submit"]').prop('disabled', true);
            $.post($form.attr('action'), $form.serialize())
                .done(function (res) {
                    if (res && res.success) {
                        modal.hide();
                        location.reload();
                    } else {
                        show(res);
                    }
                })
                .fail(function () { alert('The request failed. Please try again.'); })
                .always(function () { $submit.prop('disabled', false); });
        });
    }

    // Auto-dismiss non-error alerts after a few seconds.
    $('.alert:not(.alert-danger)').delay(4000).fadeOut(300);
});
