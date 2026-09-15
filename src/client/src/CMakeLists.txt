add_executable(gipractice-client)

ecm_add_qml_module(gipractice-client
    URI org.gipractice.client
)

target_sources(gipractice-client PRIVATE
    main.cpp
)

ecm_target_qml_sources(gipractice-client
    SOURCES
        qml/Main.qml
)

target_link_libraries(gipractice-client PRIVATE
    Qt6::Core
    Qt6::Gui
    Qt6::Network
    Qt6::Qml
    Qt6::Quick
    Qt6::QuickControls2
    Qt6::Widgets
    KF6::ConfigCore
    KF6::CoreAddons
    KF6::I18n
    KF6::IconThemes
)

install(TARGETS gipractice-client ${KDE_INSTALL_TARGETS_DEFAULT_ARGS})
install(PROGRAMS ../org.gipractice.Client.desktop DESTINATION ${KDE_INSTALL_APPDIR})
