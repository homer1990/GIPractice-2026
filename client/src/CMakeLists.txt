add_executable(gipractice-client)

target_sources(gipractice-client
    PRIVATE
        main.cpp
        clinical/AnatomyVocabulary.cpp
        clinical/AnatomyVocabulary.h
        clinical/AnatomySuggestionModel.cpp
        clinical/AnatomySuggestionModel.h
)

ecm_add_qml_module(gipractice-client
    URI net.gmanthos.gipractice
)

ecm_target_qml_sources(gipractice-client
    SOURCES
        qml/Main.qml
)

target_link_libraries(gipractice-client
    PRIVATE
        Qt6::Core
        Qt6::Gui
        Qt6::Qml
        Qt6::Quick
        Qt6::QuickControls2
        Qt6::Widgets
        KF6::CoreAddons
        KF6::I18n
)

install(TARGETS gipractice-client ${KDE_INSTALL_TARGETS_DEFAULT_ARGS})
